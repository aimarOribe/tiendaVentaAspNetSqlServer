using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCasaGrande.AplicacionWeb.Models;
using System.Diagnostics;
using AutoMapper;
using SistemaCasaGrande.AplicacionWeb.Models.ViewModels;
using SistemaCasaGrande.AplicacionWeb.Utilidades.Response;
using SistemaCasaGrande.BLL.Interfaces;
using System.Security.Claims;
using SistemaCasaGrande.Entity;

namespace SistemaCasaGrande.AplicacionWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUsuarioService _usuarioServicio;
        private readonly IMapper _mapper;

        public HomeController(IUsuarioService usuarioServicio, IMapper mapper)
        {
            _usuarioServicio = usuarioServicio;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value)
                    .SingleOrDefault();

                VMUsuario usuario = _mapper.Map<VMUsuario>(await _usuarioServicio.ObtenerPorId(int.Parse(idUsuario)));

                gResponse.Estado = true;
                gResponse.Objeto = usuario;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody] VMUsuario vmUsuario)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value)
                    .SingleOrDefault();

                Usuario usuario = _mapper.Map<Usuario>(vmUsuario);

                usuario.IdUsuario = int.Parse(idUsuario);
                
                bool resultado = await _usuarioServicio.GuardarPerfil(usuario);

                gResponse.Estado = resultado;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarClave([FromBody] VMCambiarClave vmCambiarClave)
        {
            GenericResponse<bool> gResponse = new GenericResponse<bool>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value)
                    .SingleOrDefault();

                bool resultado = await _usuarioServicio.CambiarClave(int.Parse(idUsuario), vmCambiarClave.claveActual, vmCambiarClave.claveNueva);

                gResponse.Estado = resultado;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Acceso");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}