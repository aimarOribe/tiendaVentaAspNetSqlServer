using SistemaCasaGrande.BLL.Interfaces;
using SistemaCasaGrande.DAL.Interfaces;
using SistemaCasaGrande.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaCasaGrande.BLL.Implementacion
{
    public class MenuService : IMenuService
    {
        private readonly IGenericRepository<Menu> _repositorioMenu;
        private readonly IGenericRepository<RolMenu> _repositorioRolMenu;
        private readonly IGenericRepository<Usuario> _repositorioUsuario;

        public MenuService(IGenericRepository<Menu> repositorioMenu, IGenericRepository<RolMenu> repositorioRolMenu, IGenericRepository<Usuario> repositorioUsuario)
        {
            _repositorioMenu = repositorioMenu;
            _repositorioRolMenu = repositorioRolMenu;
            _repositorioUsuario = repositorioUsuario;
        }

        public async Task<List<Menu>> ObtenerMenus(int idUsuario)
        {
            IQueryable<Usuario> tablaUsuario = await _repositorioUsuario.Consultar(u => u.IdUsuario == idUsuario);
            IQueryable<RolMenu> tablaRolMenu = await _repositorioRolMenu.Consultar();
            IQueryable<Menu> tablaMenu = await _repositorioMenu.Consultar();

            IQueryable<Menu> menuPadre = (from u in tablaUsuario
                                          join rm in tablaRolMenu on u.IdRol equals rm.IdRol
                                          join m in tablaMenu on rm.IdMenu equals m.IdMenu
                                          join mpadre in tablaMenu on m.IdMenuPadre equals mpadre.IdMenu
                                          select mpadre).Distinct().AsQueryable();

            IQueryable<Menu> menuHijos = (from u in tablaUsuario
                                          join rm in tablaRolMenu on u.IdRol equals rm.IdRol
                                          join m in tablaMenu on rm.IdMenu equals m.IdMenu
                                          where m.IdMenu != m.IdMenuPadre
                                          select m).Distinct().AsQueryable();

            List<Menu> listaMenu = (from mpadre in menuPadre
                                    select new Menu()
                                    {
                                        Descripcion = mpadre.Descripcion,
                                        Icono = mpadre.Icono,
                                        Controlador = mpadre.Controlador,
                                        PaginaAccion = mpadre.PaginaAccion,
                                        InverseIdMenuPadreNavigation = (from mhijo in menuHijos
                                                                        where mhijo.IdMenuPadre == mpadre.IdMenu
                                                                        select mhijo).ToList()
                                    }).ToList();

            return listaMenu;
        }
    }
}
