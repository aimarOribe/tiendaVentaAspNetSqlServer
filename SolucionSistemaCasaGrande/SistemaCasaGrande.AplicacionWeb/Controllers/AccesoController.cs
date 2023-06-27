using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using SistemaCasaGrande.BLL.Interfaces;
using SistemaCasaGrande.AplicacionWeb.Models.ViewModels;
using SistemaCasaGrande.Entity;

namespace SistemaCasaGrande.AplicacionWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;

        public AccesoController(IUsuarioService usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimsUser = HttpContext.User;
            if(claimsUser.Identity.IsAuthenticated) 
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult RestablecerClave()
        {
            ClaimsPrincipal claimsUser = HttpContext.User;
            if (claimsUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMUsuarioLogin usuarioLogin)
        {
            Usuario usuario_encontrado = await _usuarioServicio.ObtenerPorCredenciales(usuarioLogin.Correo, usuarioLogin.Clave);
            if(usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            ViewData["Mensaje"] = null;

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario_encontrado.IdRol.ToString()),
                new Claim("UrlFoto", usuario_encontrado.UrlFoto),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent= usuarioLogin.MantenerSesion
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(VMUsuarioLogin usuarioLogin)
        {
            try
            {
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?Clave=[clave]";
                bool resultado = await _usuarioServicio.RestablecerClave(usuarioLogin.Correo, urlPlantillaCorreo);
                if (resultado)
                {
                    ViewData["Mensaje"] = "Listo, su contraseña fue restablecida, revice su correo";
                    ViewData["MensajeError"] = null;
                }
                else
                {
                    ViewData["Mensaje"] = null;
                    ViewData["MensajeError"] = "Tenemos problemas, por favo intentelo de nuevo mas tarde";
                }
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = null;
                ViewData["MensajeError"] = ex.Message;
            }

            return View();
        }
    }

    
}
