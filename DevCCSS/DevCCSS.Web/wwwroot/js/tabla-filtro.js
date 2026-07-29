// Utilidad reutilizable para buscador dinamico + filtros + paginacion compacta (‹ pagina ›).
//
// Uso tipico en una vista:
//   <input id="buscador" data-buscar="..." />
//   <select id="filtroX" data-campo="sangre">...</select>   <-- data-campo debe coincidir con data-sangre en la fila
//   <tr class="fila-x" data-buscar="texto en minuscula" data-sangre="O+">...</tr>
//   <ul id="paginacion"></ul>
//   <div id="sinResultados" style="display:none;">Sin resultados</div>
//
//   initTablaFiltro({ filas: '.fila-x', buscador: '#buscador', filtros: ['#filtroX'],
//                      contenedorPaginacion: '#paginacion', sinResultados: '#sinResultados' });
function initTablaFiltro(opts) {
    const porPagina = opts.porPagina || 10;
    let paginaActual = 1;

    const buscador = opts.buscador ? document.querySelector(opts.buscador) : null;
    const filtros = (opts.filtros || []).map(sel => document.querySelector(sel)).filter(Boolean);
    const filas = Array.from(document.querySelectorAll(opts.filas));
    const paginacion = opts.contenedorPaginacion ? document.querySelector(opts.contenedorPaginacion) : null;
    const sinResultados = opts.sinResultados ? document.querySelector(opts.sinResultados) : null;

    function filasVisibles() {
        const texto = buscador ? buscador.value.toLowerCase().trim() : '';
        return filas.filter(f => {
            const coincideTexto = !texto || (f.dataset.buscar || '').includes(texto);
            const coincideFiltros = filtros.every(sel => {
                const valor = sel.value;
                const campo = sel.dataset.campo;
                return !valor || !campo || f.dataset[campo] === valor;
            });
            return coincideTexto && coincideFiltros;
        });
    }

    function render() {
        const visibles = filasVisibles();
        filas.forEach(f => f.style.display = 'none');
        if (sinResultados) sinResultados.style.display = visibles.length === 0 ? 'block' : 'none';

        const inicio = (paginaActual - 1) * porPagina;
        visibles.slice(inicio, inicio + porPagina).forEach(f => f.style.display = '');

        renderPaginacion(visibles.length);
    }

    function renderPaginacion(total) {
        if (!paginacion) return;
        const totalPaginas = Math.max(1, Math.ceil(total / porPagina));
        if (paginaActual > totalPaginas) paginaActual = totalPaginas;
        paginacion.innerHTML = '';
        if (totalPaginas <= 1) return;

        const liPrev = document.createElement('li');
        liPrev.className = 'page-item' + (paginaActual === 1 ? ' disabled' : '');
        liPrev.innerHTML = '<a class="page-link" href="#">&lsaquo;</a>';
        liPrev.addEventListener('click', (e) => {
            e.preventDefault();
            if (paginaActual > 1) { paginaActual--; render(); }
        });

        const liActual = document.createElement('li');
        liActual.className = 'page-item disabled';
        liActual.innerHTML = `<span class="page-link">${paginaActual} / ${totalPaginas}</span>`;

        const liNext = document.createElement('li');
        liNext.className = 'page-item' + (paginaActual === totalPaginas ? ' disabled' : '');
        liNext.innerHTML = '<a class="page-link" href="#">&rsaquo;</a>';
        liNext.addEventListener('click', (e) => {
            e.preventDefault();
            if (paginaActual < totalPaginas) { paginaActual++; render(); }
        });

        paginacion.appendChild(liPrev);
        paginacion.appendChild(liActual);
        paginacion.appendChild(liNext);

        // Ir directo a una pagina especifica (se reconstruye en cada render)
        if (totalPaginas > 1) {
            const liIr = document.createElement('li');
            liIr.className = 'page-item ms-2';
            liIr.innerHTML = `
                <div class="input-group input-group-sm" style="width: 110px;">
                    <input type="number" min="1" max="${totalPaginas}" class="form-control" placeholder="Pag.">
                    <button class="btn btn-outline-secondary" type="button">Ir</button>
                </div>`;

            const input = liIr.querySelector('input');
            const boton = liIr.querySelector('button');

            const irAPagina = () => {
                let destino = parseInt(input.value, 10);
                if (!destino || destino < 1) destino = 1;
                if (destino > totalPaginas) destino = totalPaginas;
                paginaActual = destino;
                render();
            };

            boton.addEventListener('click', irAPagina);
            input.addEventListener('keydown', (e) => {
                if (e.key === 'Enter') { e.preventDefault(); irAPagina(); }
            });

            paginacion.appendChild(liIr);
        }
    }

    if (buscador) buscador.addEventListener('input', () => { paginaActual = 1; render(); });
    filtros.forEach(f => f.addEventListener('change', () => { paginaActual = 1; render(); }));

    render();
}
