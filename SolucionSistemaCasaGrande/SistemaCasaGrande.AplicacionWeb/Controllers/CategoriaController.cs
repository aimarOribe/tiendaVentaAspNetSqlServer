using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaCasaGrande.AplicacionWeb.Models.ViewModels;
using SistemaCasaGrande.AplicacionWeb.Utilidades.Response;
using SistemaCasaGrande.BLL.Interfaces;
using SistemaCasaGrande.Entity;
using Microsoft.AspNetCore.Authorization;

namespace SistemaCasaGrande.AplicacionWeb.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICategoriaService _categoriaServicio;

        public CategoriaController(IMapper mapper, ICategoriaService categoriaServicio)
        {
            _mapper = mapper;
            _categoriaServicio = categoriaServicio;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMCategoria> vmCategoria = _mapper.Map<List<VMCategoria>>(await _categoriaServicio.Lista()); 
            return StatusCode(StatusCodes.Status200OK, new {data = vmCategoria});
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMCategoria categoria)
        {
            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try
            {
                Categoria categoriaCreada = await _categoriaServicio.Crear(_mapper.Map<Categoria>(categoria));
                categoria = _mapper.Map<VMCategoria>(categoriaCreada);

                gResponse.Estado = true;
                gResponse.Objeto = categoria;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                throw;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMCategoria categoria)
        {
            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try
            {
                Categoria categoriaEditada = await _categoriaServicio.Editar(_mapper.Map<Categoria>(categoria));
                categoria = _mapper.Map<VMCategoria>(categoriaEditada);

                gResponse.Estado = true;
                gResponse.Objeto = categoria;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                throw;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idCategoria)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _categoriaServicio.Eliminar(idCategoria);
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
