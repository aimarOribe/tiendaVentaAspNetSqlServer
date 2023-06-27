using SistemaCasaGrande.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaCasaGrande.BLL.Interfaces
{
    public interface IProductoService
    {
        Task<List<Producto>> Lista();
        Task<Producto> Crear(Producto producto, Stream imagen = null, string nombreImagen = "");
        Task<Producto> Editar(Producto producto, Stream imagen = null, string nombreImagen = "");
        Task<bool> Eliminar(int idProducto);
    }
}
