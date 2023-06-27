using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using SistemaCasaGrande.AplicacionWeb.Models.ViewModels;
using SistemaCasaGrande.AplicacionWeb.Utilidades.Response;
using SistemaCasaGrande.BLL.Interfaces;
using SistemaCasaGrande.Entity;
using Microsoft.AspNetCore.Authorization;

namespace SistemaCasaGrande.AplicacionWeb.Controllers
{
    [Authorize]
    public class NegocioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioServicio;

        public NegocioController(IMapper mapper, INegocioService negocioServicio)
        {
            _mapper = mapper;
            _negocioServicio = negocioServicio;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();

            try
            {
                VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioServicio.Obtener());
                gResponse.Estado = true;
                gResponse.Objeto = vmNegocio;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                throw;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambios([FromForm] IFormFile logo, [FromForm] string negocio)
        {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();

            try
            {
                VMNegocio vmNegocio = JsonConvert.DeserializeObject<VMNegocio>(negocio);

                string nombreLogo = "";
                Stream logoStream = null;

                if(logo != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(logo.FileName);
                    nombreLogo = string.Concat(nombreEnCodigo, extension);
                    logoStream = logo.OpenReadStream();
                }

                Negocio negocioEditado = await _negocioServicio.GuardarCambios(_mapper.Map<Negocio>(vmNegocio), logoStream, nombreLogo);

                vmNegocio = _mapper.Map<VMNegocio>(negocioEditado);

                gResponse.Estado = true;
                gResponse.Objeto = vmNegocio;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                throw;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
