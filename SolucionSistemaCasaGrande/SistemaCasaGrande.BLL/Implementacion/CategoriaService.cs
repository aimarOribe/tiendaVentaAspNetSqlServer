using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaCasaGrande.BLL.Interfaces;
using SistemaCasaGrande.DAL.Interfaces;
using SistemaCasaGrande.Entity;

namespace SistemaCasaGrande.BLL.Implementacion
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IGenericRepository<Categoria> _repository;

        public CategoriaService(IGenericRepository<Categoria> repository)
        {
            _repository = repository;
        }

        public async Task<List<Categoria>> Lista()
        {
            IQueryable<Categoria> query = await _repository.Consultar();
            return query.ToList();
        }

        public async Task<Categoria> Crear(Categoria categoria)
        {
            try
            {
                Categoria categoriaCreada = await _repository.Crear(categoria);
                if(categoriaCreada.IdCategoria == 0)             
                    throw new TaskCanceledException("No se pudo crear la categoria");              
                return categoriaCreada;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Categoria> Editar(Categoria categoria)
        {
            try
            {
                Categoria categoriaEncontrada = await _repository.Obtener(c => c.IdCategoria == categoria.IdCategoria);
                categoriaEncontrada.Descripcion = categoria.Descripcion;
                categoriaEncontrada.EsActivo = categoria.EsActivo;
                bool respuesta = await _repository.Editar(categoriaEncontrada);
                if (!respuesta)
                    throw new TaskCanceledException("No se logro editar la categoria");
                return categoriaEncontrada;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idCategoria)
        {
            try
            {
                Categoria categoriaEncontrada = await _repository.Obtener(c => c.IdCategoria == idCategoria);
                if(categoriaEncontrada == null)
                {
                    throw new TaskCanceledException("No existe la categoria");
                }
                bool respuesta = await _repository.Eliminar(categoriaEncontrada);
                if (!respuesta) throw new TaskCanceledException("No se logro eliminar la categoria");
                return respuesta;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
