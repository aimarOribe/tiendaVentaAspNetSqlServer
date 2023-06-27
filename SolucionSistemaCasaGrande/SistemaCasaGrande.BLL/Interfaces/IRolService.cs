using SistemaCasaGrande.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaCasaGrande.BLL.Interfaces
{
    public interface IRolService
    {
        Task<List<Rol>> Lista();
    }
}
