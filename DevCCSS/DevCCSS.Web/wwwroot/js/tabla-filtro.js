// Traduccion estandar de DataTables al espanol, incrustada (no depende de
// bajar un JSON remoto, que en algunos entornos no resuelve y deja la
// tabla sin inicializar).
const DATATABLE_IDIOMA_ES = {
    emptyTable: 'No hay datos disponibles',
    info: 'Mostrando _START_ a _END_ de _TOTAL_ registros',
    infoEmpty: 'Mostrando 0 a 0 de 0 registros',
    infoFiltered: '(filtrado de _MAX_ registros totales)',
    lengthMenu: 'Mostrar _MENU_ registros',
    loadingRecords: 'Cargando...',
    processing: 'Procesando...',
    search: 'Buscar:',
    zeroRecords: 'No se encontraron resultados',
    paginate: { first: 'Primero', last: 'Último', next: 'Siguiente', previous: 'Anterior' }
};

// Envuelve una tabla HTML existente en un DataTable (paginacion, numeracion
// y "sin resultados" estandar), conservando el buscador y los filtros
// personalizados que ya trae cada vista.
//
// Misma firma que antes para no tener que tocar las vistas que ya la usan:
//   initTablaFiltro({ filas: '.fila-x', buscador: '#buscador', filtros: ['#filtroX'],
//                      contenedorPaginacion: '#paginacion', sinResultados: '#sinResultados' });
//
// Cada fila debe seguir trayendo sus atributos data-buscar / data-campo tal
// cual ya los trae hoy; el buscador y los filtros los siguen leyendo de ahi
// (en vez del buscador nativo de DataTables) para mantener exactamente el
// mismo comportamiento de busqueda que tenia la version anterior.
function initTablaFiltro(opts) {
    if (!window.DataTable) return;

    const buscador = opts.buscador ? document.querySelector(opts.buscador) : null;
    const filtros = (opts.filtros || []).map(sel => document.querySelector(sel)).filter(Boolean);

    // Se ubica la tabla anclado al buscador (siempre presente en el markup,
    // haya o no filas) en vez de a una fila de ejemplo -- si la lista viene
    // vacia (0 resultados) no hay ninguna fila de la que partir.
    let tabla = null;
    if (buscador) {
        const contenedor = buscador.closest('.tab-pane') || buscador.closest('main') || document;
        tabla = contenedor.querySelector('table');
    }
    if (!tabla) {
        const filaSample = document.querySelector(opts.filas);
        tabla = filaSample ? filaSample.closest('table') : null;
    }
    if (!tabla) return;

    // La paginacion y el "sin resultados" manuales ya no hacen falta: los trae el datatable.
    if (opts.contenedorPaginacion) {
        const cont = document.querySelector(opts.contenedorPaginacion);
        const nav = cont ? cont.closest('nav') : null;
        (nav || cont)?.remove();
    }
    if (opts.sinResultados) {
        document.querySelector(opts.sinResultados)?.remove();
    }

    const columnas = tabla.querySelectorAll('thead th').length;

    // Se oculta el buscador nativo del datatable (topEnd) porque la vista ya
    // trae el suyo propio; se conserva el resto estandar (largo de pagina,
    // info y paginacion).
    const dt = new DataTable(tabla, {
        language: DATATABLE_IDIOMA_ES,
        pageLength: opts.porPagina || 10,
        columnDefs: columnas > 0 ? [{ targets: columnas - 1, orderable: false, searchable: false }] : [],
        order: [],
        layout: { topStart: 'pageLength', topEnd: null, bottomStart: 'info', bottomEnd: 'paging' }
    });

    // Filtro de texto/desplegables personalizado: revisa los mismos atributos
    // data-* que ya trae cada <tr>, en vez del buscador nativo de DataTables
    // (que solo mira el texto visible de las celdas).
    DataTable.ext.search.push(function (settings, dataText, dataIndex) {
        if (settings.nTable !== tabla) return true;

        const fila = dt.row(dataIndex).node();
        if (!fila) return true;

        const texto = buscador ? buscador.value.toLowerCase().trim() : '';
        if (texto && !(fila.dataset.buscar || '').includes(texto)) return false;

        for (const sel of filtros) {
            const valor = sel.value;
            const campo = sel.dataset.campo;
            if (valor && campo && fila.dataset[campo] !== valor) return false;
        }
        return true;
    });

    if (buscador) buscador.addEventListener('input', () => dt.draw());
    filtros.forEach(f => f.addEventListener('change', () => dt.draw()));
}

// Vuelve buscable un <select> normal (ej. el paciente en un formulario):
// un campo de texto arriba filtra las <option> por su texto visible, sin
// tocar el value que termina enviandose (sigue siendo un <select> nativo).
// Mismo patron ya usado para el filtro medico/especialidad de Citas, pero
// generico para poder reutilizarlo en cualquier <select> largo.
function initSelectBuscable(selectorSelect, selectorInput) {
    const select = document.querySelector(selectorSelect);
    const input = document.querySelector(selectorInput);
    if (!select || !input) return;

    const opciones = Array.from(select.options);

    input.addEventListener('input', () => {
        const texto = input.value.toLowerCase().trim();
        opciones.forEach(opt => {
            if (!opt.value) { opt.hidden = false; return; }
            opt.hidden = texto !== '' && !opt.text.toLowerCase().includes(texto);
        });
        if (select.selectedOptions[0] && select.selectedOptions[0].hidden) {
            select.value = '';
        }
    });
}
