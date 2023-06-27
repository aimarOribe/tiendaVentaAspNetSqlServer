using Azure;
using Microsoft.EntityFrameworkCore;
using SistemaCasaGrande.BLL.Interfaces;
using SistemaCasaGrande.DAL.Interfaces;
using SistemaCasaGrande.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace SistemaCasaGrande.BLL.Implementacion
{
    public class ProductoService : IProductoService
    {
        private readonly IGenericRepository<Producto> _repositorio;
        private readonly IFireBaseService _fireBaseServicio;

        public ProductoService(IGenericRepository<Producto> repository, IFireBaseService fireBaseServicio)
        {
            _repositorio = repository;
            _fireBaseServicio = fireBaseServicio;
        }

        public async Task<List<Producto>> Lista()
        {
            IQueryable<Producto> query = await _repositorio.Consultar();
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }
        public async Task<Producto> Crear(Producto producto, Stream imagen = null, string nombreImagen = "")
        {
            Producto productoExiste = await _repositorio.Obtener(p => p.CodigoBarra == producto.CodigoBarra);
            if(productoExiste != null)         
                throw new TaskCanceledException("El codigo de barra ya existe");
            try
            {
                producto.NombreImagen = nombreImagen;
                if(imagen != null)
                {
                    string urlImagen = await _fireBaseServicio.subirStorage(imagen, "carpeta_producto", nombreImagen);
                    producto.UrlImagen = urlImagen;
                }
                Producto productoCreado = await _repositorio.Crear(producto);

                if(productoCreado.IdProducto == 0)               
                    throw new TaskCanceledException("No se pudo crear el producto");

                IQueryable<Producto> query = await _repositorio.Consultar(p => p.IdProducto == producto.IdProducto);
            
                productoCreado = query.Include(c => c.IdCategoriaNavigation).First();

                return productoCreado;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task<Producto> Editar(Producto producto, Stream imagen = null, string nombreImagen = "")
        {
            Producto productoExiste = await _repositorio.Obtener(p => p.CodigoBarra == producto.CodigoBarra && p.IdProducto != producto.IdProducto);
            if(productoExiste != null)
                throw new TaskCanceledException("El codigo de barra ya existe en un producto");

            try
            {
                IQueryable<Producto> queryProducto = await _repositorio.Consultar(p => p.IdProducto == producto.IdProducto);
                Producto productoParaEditar = queryProducto.First();
                productoParaEditar.CodigoBarra = producto.CodigoBarra;
                productoParaEditar.Marca = producto.Marca;
                productoParaEditar.Descripcion = producto.Descripcion;
                productoParaEditar.Precio = producto.Precio;
                productoParaEditar.Stock = producto.Stock;
                productoParaEditar.IdCategoria = producto.IdCategoria;
                productoParaEditar.EsActivo = producto.EsActivo;

                if(productoParaEditar.NombreImagen == "")
                {
                    productoParaEditar.NombreImagen = nombreImagen;
                }

                if(imagen != null)
                {
                    string urlImagen = await _fireBaseServicio.subirStorage(imagen, "carpeta_producto", productoParaEditar.NombreImagen);
                    productoParaEditar.UrlImagen = urlImagen;
                }

                bool respuesta = await _repositorio.Editar(productoParaEditar);

                if(!respuesta)
                    throw new TaskCanceledException("No se pudo editar el producto");

                Producto productoEditado = queryProducto.Include(c => c.IdCategoriaNavigation).First();

                return productoEditado;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<bool> Eliminar(int idProducto)
        {
            try
            {
                Producto productoEncontrado = await _repositorio.Obtener(p => p.IdProducto == idProducto);

                if(productoEncontrado == null)
                    throw new TaskCanceledException("El producto no existe");

                string nombreImagen = productoEncontrado.NombreImagen;

                bool respuesta = await _repositorio.Eliminar(productoEncontrado);

                if (respuesta)
                    await _fireBaseServicio.eliminarStorage("carpeta_producto", nombreImagen);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        } 
    }
}
