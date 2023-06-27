using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaCasaGrande.BLL.Interfaces
{
    public interface IDashBoardService
    {
        Task<int> totalVentasUltimaSemana();
        
        Task<string> totalIngresosUltimaSemana();
        
        Task<int> totalProductos();
        
        Task<int> totalCategorias();
        Task<Dictionary<string, int>> ventasUltimaSemana();

        Task<Dictionary<string, int>> productosTopUltimaSemana();
    }
}
