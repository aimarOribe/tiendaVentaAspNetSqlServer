using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaCasaGrande.Entity;

namespace SistemaCasaGrande.BLL.Interfaces
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> Lista();
        Task<Categoria> Crear(Categoria categoria);
        Task<Categoria> Editar(Categoria categoria);
        Task<bool> Eliminar(int idCategoria);
    }
}
