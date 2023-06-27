using Microsoft.AspNetCore.Mvc;
using SistemaCasaGrande.BLL.Interfaces;
using AutoMapper;
using SistemaCasaGrande.AplicacionWeb.Models.ViewModels;
using SistemaCasaGrande.AplicacionWeb.Utilidades.Response;
using SistemaCasaGrande.Entity;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SistemaCasaGrande.AplicacionWeb.Controllers
{
    [Authorize]
    public class VentaController : Controller
    {

        private readonly ITipoDocumentoVentaService _tipoDocumentoVentaServicio;
        private readonly IVentaService _ventaServicio;
        private readonly IMapper _mapper;
        private readonly IConverter _converter;

        public VentaController(ITipoDocumentoVentaService tipoDocumentoVentaServicio, IVentaService ventaServicio, IMapper mapper, IConverter converter)
        {
            _tipoDocumentoVentaServicio = tipoDocumentoVentaServicio;
            _ventaServicio = ventaServicio;
            _mapper = mapper;
            _converter = converter;
        }

        public IActionResult NuevaVenta()
        {
            return View();
        }

        public IActionResult HistorialVenta()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoDocumentoVenta()
        {
            List<VMTipoDocumentoVenta> vmListaTipoDocumentoVenta = _mapper.Map<List<VMTipoDocumentoVenta>>(await _tipoDocumentoVentaServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaTipoDocumentoVenta);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string busqueda)
        {
            List<VMProducto> vmListaProductos = _mapper.Map<List<VMProducto>>(await _ventaServicio.ObtenerProductos(busqueda));
            return StatusCode(StatusCodes.Status200OK, vmListaProductos);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VMVenta venta)
        {
            GenericResponse<VMVenta> gResponse = new GenericResponse<VMVenta>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value)
                    .SingleOrDefault();

                venta.IdUsuario = int.Parse(idUsuario);
                Venta ventaCreada = await _ventaServicio.Registrar(_mapper.Map<Venta>(venta));
                venta = _mapper.Map<VMVenta>(ventaCreada);

                gResponse.Estado = true;
                gResponse.Objeto = venta;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                return StatusCode(StatusCodes.Status200OK, gResponse);
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            List<VMVenta> vmHistorialVenta = _mapper.Map<List<VMVenta>>(await _ventaServicio.Historial(numeroVenta, fechaInicio, fechaFin));
            return StatusCode(StatusCodes.Status200OK, vmHistorialVenta);
        }

        public IActionResult MostrarPDFVenta(string numeroVenta)
        {
            string urlPlantillaVista = $"{this.Request.Scheme}:://{this.Request.Host}/Plantilla/PDFVenta?numeroVenta={numeroVenta}";
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        Page = urlPlantillaVista
                    }
                }
            };

            var archivoPDF = _converter.Convert(pdf);

            return File(archivoPDF, "application/pdf");
        }
    }
}
