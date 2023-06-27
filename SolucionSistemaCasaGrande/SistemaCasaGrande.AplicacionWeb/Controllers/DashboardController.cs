using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCasaGrande.AplicacionWeb.Models.ViewModels;
using SistemaCasaGrande.AplicacionWeb.Utilidades.Response;
using SistemaCasaGrande.BLL.Interfaces;

namespace SistemaCasaGrande.AplicacionWeb.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashBoardService _dashBoardServicio;

        public DashboardController(IDashBoardService dashBoardServicio)
        {
            _dashBoardServicio = dashBoardServicio;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashboard> gResponse = new GenericResponse<VMDashboard>();

            try
            {
                VMDashboard vmDashBoard = new VMDashboard();
                vmDashBoard.TotalVentas = await _dashBoardServicio.totalVentasUltimaSemana();
                vmDashBoard.TotalIngresos = await _dashBoardServicio.totalIngresosUltimaSemana();
                vmDashBoard.TotalProductos = await _dashBoardServicio.totalProductos();
                vmDashBoard.TotalCategorias = await _dashBoardServicio.totalCategorias();

                List<VMVentasSemana> listaVentaSemana = new List<VMVentasSemana>();
                List<VMProductosSemana> listaProductosSemana = new List<VMProductosSemana>();

                foreach (KeyValuePair<string, int> item in await _dashBoardServicio.ventasUltimaSemana())
                {
                    listaVentaSemana.Add(new VMVentasSemana()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                }

                foreach (KeyValuePair<string, int> item in await _dashBoardServicio.productosTopUltimaSemana())
                {
                    listaProductosSemana.Add(new VMProductosSemana()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value
                    });
                }

                vmDashBoard.VentasUltimaSemana = listaVentaSemana;
                vmDashBoard.ProductosTopUltimaSemana = listaProductosSemana;

                gResponse.Estado = true;
                gResponse.Objeto = vmDashBoard;

                return StatusCode(StatusCodes.Status200OK, gResponse);
            } 
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                return StatusCode(StatusCodes.Status200OK, gResponse);
            }
        }
    }
}
