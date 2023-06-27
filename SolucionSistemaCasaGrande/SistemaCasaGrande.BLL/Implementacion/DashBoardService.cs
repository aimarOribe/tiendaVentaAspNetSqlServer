using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaCasaGrande.BLL.Interfaces;
using System.Globalization;
using SistemaCasaGrande.DAL.Interfaces;
using SistemaCasaGrande.Entity;

namespace SistemaCasaGrande.BLL.Implementacion
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IVentaRepository _repositorioVenta;
        private readonly IGenericRepository<DetalleVenta> _repositorioDetalleVenta;
        private readonly IGenericRepository<Categoria> _repositorioCategoria;
        private readonly IGenericRepository<Producto> _repositorioProducto;
        private DateTime fechaInicio = DateTime.Now;

        public DashBoardService(IVentaRepository repositorioVenta, IGenericRepository<DetalleVenta> repositorioDetalleVenta, IGenericRepository<Categoria> repositorioCategoria, IGenericRepository<Producto> repositorioProducto)
        {
            _repositorioVenta = repositorioVenta;
            _repositorioDetalleVenta = repositorioDetalleVenta;
            _repositorioCategoria = repositorioCategoria;
            _repositorioProducto = repositorioProducto;
            fechaInicio = fechaInicio.AddDays(-7);
        }

        public async Task<int> totalVentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.FechaRegistro.Value.Date >= fechaInicio.Date);
                int total = query.Count();
                return total;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> totalIngresosUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.FechaRegistro.Value.Date >= fechaInicio.Date);
                decimal resultado = query
                    .Select(v => v.Total)
                    .Sum(v => v.Value);
                return Convert.ToString(resultado, new CultureInfo("es-PE"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> totalProductos()
        {
            try
            {
                IQueryable<Producto> query = await _repositorioProducto.Consultar();
                int total = query.Count();
                return total;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> totalCategorias()
        {
            try
            {
                IQueryable<Categoria> query = await _repositorioCategoria.Consultar();
                int total = query.Count();
                return total;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> ventasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _repositorioVenta.Consultar(
                    v => v.FechaRegistro.Value.Date >= fechaInicio.Date);

                Dictionary<string, int> resultado = query
                    .GroupBy(v => v.FechaRegistro.Value.Date)
                    .OrderByDescending(g => g.Key)
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: e => e.total);
                
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> productosTopUltimaSemana()
        {
            try
            {
                IQueryable<DetalleVenta> query = await _repositorioDetalleVenta.Consultar();
                Dictionary<string, int> resultado = query
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio.Date)
                    .GroupBy(dv => dv.DescripcionProducto)
                    .OrderByDescending(g => g.Count())
                    .Select(dv => new { producto = dv.Key, total = dv.Count() })
                    .Take(4)
                    .ToDictionary(keySelector: r => r.producto, elementSelector: e => e.total);

                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
