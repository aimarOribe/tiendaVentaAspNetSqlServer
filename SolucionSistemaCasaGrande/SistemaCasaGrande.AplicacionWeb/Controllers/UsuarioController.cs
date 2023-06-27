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
    public class UsuarioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioServicio;
        private readonly IRolService _rolServicio;

        public UsuarioController(IMapper mapper, IUsuarioService usuarioServicio, IRolService rolServicio)
        {
            _mapper = mapper;
            _usuarioServicio = usuarioServicio;
            _rolServicio = rolServicio;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(await _rolServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);
        }

        [HttpGet]
        public async Task<IActionResult> ListaUsuarios()
        {
            List<VMUsuario> vmListaUsuarios = _mapper.Map<List<VMUsuario>>(await _usuarioServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmListaUsuarios });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string usuario)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(usuario);
                string nombreFoto = "";
                Stream fotoStream = null;

                if(foto != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombreEnCodigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                Usuario usuarioCreado = await _usuarioServicio.Crear(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);

                vmUsuario = _mapper.Map<VMUsuario>(usuarioCreado);

                gResponse.Estado = true;
                gResponse.Objeto = vmUsuario;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                return StatusCode(StatusCodes.Status200OK, gResponse);

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string usuario)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(usuario);
                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombreEnCodigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuarioEditado = await _usuarioServicio.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);

                vmUsuario = _mapper.Map<VMUsuario>(usuarioEditado);

                gResponse.Estado = true;
                gResponse.Objeto = vmUsuario;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                return StatusCode(StatusCodes.Status200OK, gResponse);
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _usuarioServicio.Eliminar(idUsuario);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                return StatusCode(StatusCodes.Status200OK, gResponse);
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
