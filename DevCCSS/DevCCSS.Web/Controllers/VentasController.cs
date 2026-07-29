using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador,Facturacion")]
    public class VentasController : Controller
    {
        private readonly VentaClient _ventas;
        public VentasController(VentaClient ventas) => _ventas = ventas;

        public async Task<IActionResult> Index()
        {
            var lista = await _ventas.ListarAsync();
            return View(lista);
        }

        public async Task<IActionResult> Details(int id)
        {
            var venta = await _ventas.ObtenerPorIdAsync(id);
            if (venta is null) return NotFound();
            return View(venta);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Productos = await _ventas.ListarProductosAsync();
            return View(new CrearVentaDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearVentaDto model)
        {
            if (model.Items is null || model.Items.Count == 0)
                ModelState.AddModelError("", "Agregue al menos un producto a la venta.");

            if (!ModelState.IsValid)
            {
                ViewBag.Productos = await _ventas.ListarProductosAsync();
                return View(model);
            }

            try
            {
                var r = await _ventas.RegistrarAsync(model);

                if (!r.Ok)
                {
                    if (r.CodigoSalida == 2)
                        ModelState.AddModelError("PacienteIdentificacion", r.Mensaje);
                    else
                        ModelState.AddModelError("", r.Mensaje);

                    ViewBag.Productos = await _ventas.ListarProductosAsync();
                    return View(model);
                }

                TempData["Ok"] = $"{r.Mensaje} Factura #{r.IdFacturaGenerada}.";
                return RedirectToAction(nameof(Details), new { id = r.IdFacturaGenerada });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error de comunicacion con el servicio de ventas: " + ex.Message);
                ViewBag.Productos = await _ventas.ListarProductosAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> ExportarExcel(int id)
        {
            var venta = await _ventas.ObtenerPorIdAsync(id);
            if (venta is null) return NotFound();

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var hoja = workbook.Worksheets.Add("Factura");

            hoja.Cell("A1").Value = "Hospital DevCCSS";
            hoja.Cell("A1").Style.Font.Bold = true;
            hoja.Cell("A1").Style.Font.FontSize = 16;
            hoja.Cell("A2").Value = $"Factura #{venta.IdFactura}";
            hoja.Cell("A3").Value = $"Fecha: {venta.FechaFactura:dd/MM/yyyy HH:mm}";
            hoja.Cell("A4").Value = $"Cliente: {venta.ClienteNombreCompleto ?? "Cliente ocasional"}";
            hoja.Cell("A5").Value = $"Atendido por: {venta.EmpleadoNombreCompleto ?? "-"}";
            hoja.Cell("A6").Value = $"Método de pago: {venta.MetodoPago}";

            int filaEncabezado = 8;
            hoja.Cell(filaEncabezado, 1).Value = "#";
            hoja.Cell(filaEncabezado, 2).Value = "Producto";
            hoja.Cell(filaEncabezado, 3).Value = "Cantidad";
            hoja.Cell(filaEncabezado, 4).Value = "Precio unitario";
            hoja.Cell(filaEncabezado, 5).Value = "Subtotal";

            var rangoEncabezado = hoja.Range(filaEncabezado, 1, filaEncabezado, 5);
            rangoEncabezado.Style.Font.Bold = true;
            rangoEncabezado.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;

            int fila = filaEncabezado + 1;
            int numero = 1;
            foreach (var d in venta.Detalle)
            {
                hoja.Cell(fila, 1).Value = numero;
                hoja.Cell(fila, 2).Value = d.NombreProducto;
                hoja.Cell(fila, 3).Value = d.Cantidad;
                hoja.Cell(fila, 4).Value = d.PrecioUnitario;
                hoja.Cell(fila, 4).Style.NumberFormat.Format = "₡#,##0.00";
                hoja.Cell(fila, 5).Value = d.SubtotalLinea;
                hoja.Cell(fila, 5).Style.NumberFormat.Format = "₡#,##0.00";
                fila++;
                numero++;
            }

            fila += 1;
            hoja.Cell(fila, 4).Value = "Subtotal";
            hoja.Cell(fila, 5).Value = venta.Subtotal;
            hoja.Cell(fila, 5).Style.NumberFormat.Format = "₡#,##0.00";
            fila++;

            hoja.Cell(fila, 4).Value = "Descuento";
            hoja.Cell(fila, 5).Value = venta.Descuento;
            hoja.Cell(fila, 5).Style.NumberFormat.Format = "₡#,##0.00";
            fila++;

            hoja.Cell(fila, 4).Value = "Impuestos (13%)";
            hoja.Cell(fila, 5).Value = venta.Impuestos;
            hoja.Cell(fila, 5).Style.NumberFormat.Format = "₡#,##0.00";
            fila++;

            hoja.Cell(fila, 4).Value = "Total";
            hoja.Cell(fila, 4).Style.Font.Bold = true;
            hoja.Cell(fila, 5).Value = venta.Total;
            hoja.Cell(fila, 5).Style.NumberFormat.Format = "₡#,##0.00";
            hoja.Cell(fila, 5).Style.Font.Bold = true;

            hoja.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Factura_{venta.IdFactura}.xlsx");
        }

        public async Task<IActionResult> ExportarPdf(int id)
        {
            var venta = await _ventas.ObtenerPorIdAsync(id);
            if (venta is null) return NotFound();

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Hospital DevCCSS").FontSize(20).Bold();
                        col.Item().Text($"Factura #{venta.IdFactura}").FontSize(14);
                        col.Item().PaddingTop(5).LineHorizontal(1);
                    });

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Cliente:").Bold();
                                c.Item().Text(venta.ClienteNombreCompleto ?? "Cliente ocasional");
                                if (!string.IsNullOrEmpty(venta.ClienteIdentificacion))
                                    c.Item().Text($"Cédula: {venta.ClienteIdentificacion}").FontSize(9);
                            });
                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text($"Fecha: {venta.FechaFactura:dd/MM/yyyy HH:mm}");
                                c.Item().Text($"Atendido por: {venta.EmpleadoNombreCompleto ?? "-"}");
                                c.Item().Text($"Método de pago: {venta.MetodoPago}");
                            });
                        });

                        col.Item().PaddingTop(15).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columnas =>
                            {
                                columnas.ConstantColumn(25);
                                columnas.RelativeColumn(3);
                                columnas.RelativeColumn(1);
                                columnas.RelativeColumn(1);
                                columnas.RelativeColumn(1);
                            });

                            tabla.Header(header =>
                            {
                                header.Cell().Text("#").Bold();
                                header.Cell().Text("Producto").Bold();
                                header.Cell().AlignRight().Text("Cant.").Bold();
                                header.Cell().AlignRight().Text("Precio").Bold();
                                header.Cell().AlignRight().Text("Subtotal").Bold();
                                header.Cell().ColumnSpan(5).PaddingTop(3).BorderBottom(1);
                            });

                            int numero = 1;
                            foreach (var d in venta.Detalle)
                            {
                                tabla.Cell().Text(numero.ToString());
                                tabla.Cell().Text(d.NombreProducto);
                                tabla.Cell().AlignRight().Text(d.Cantidad.ToString());
                                tabla.Cell().AlignRight().Text($"₡{d.PrecioUnitario:N2}");
                                tabla.Cell().AlignRight().Text($"₡{d.SubtotalLinea:N2}");
                                numero++;
                            }
                        });

                        col.Item().PaddingTop(15).AlignRight().Column(c =>
                        {
                            c.Item().Text($"Subtotal: ₡{venta.Subtotal:N2}");
                            c.Item().Text($"Descuento: -₡{venta.Descuento:N2}");
                            c.Item().Text($"Impuestos (13%): +₡{venta.Impuestos:N2}");
                            c.Item().PaddingTop(5).BorderTop(1).PaddingTop(3)
                                .Text($"Total: ₡{venta.Total:N2}").FontSize(14).Bold();
                        });
                    });

                    page.Footer().AlignCenter().Text("Hospital DevCCSS | GRUPO 2").FontSize(9);
                });
            });

            var bytes = documento.GeneratePdf();
            return File(bytes, "application/pdf", $"Factura_{venta.IdFactura}.pdf");
        }
    }
}