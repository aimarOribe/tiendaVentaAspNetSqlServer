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
    public class TipoDocumentoVentaService : ITipoDocumentoVentaService
    {

        private readonly IGenericRepository<TipoDocumentoVenta> _repositorio;

        public TipoDocumentoVentaService(IGenericRepository<TipoDocumentoVenta> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<TipoDocumentoVenta>> Lista()
        {
            IQueryable<TipoDocumentoVenta> query = await _repositorio.Consultar();
            return query.ToList();
        }
    }
}
