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
    public class NegocioService : INegocioService
    {
        private readonly IGenericRepository<Negocio> _repositorio;
        private readonly IFireBaseService _fireBaseServicio;

        public NegocioService(IGenericRepository<Negocio> repositorio, IFireBaseService fireBaseServicio)
        {
            _repositorio = repositorio;
            _fireBaseServicio = fireBaseServicio;
        }

        public async Task<Negocio> Obtener()
        {
            try
            {
                Negocio negocioEncontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);
                return negocioEncontrado;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Negocio> GuardarCambios(Negocio negocio, Stream logo = null, string nombreLogo = "")
        {
            try
            {
                Negocio negocioEncontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);
                negocioEncontrado.NumeroDocumento = negocio.NumeroDocumento;
                negocioEncontrado.Nombre = negocio.Nombre;
                negocioEncontrado.Correo = negocio.Correo;
                negocioEncontrado.Direccion = negocio.Direccion;
                negocioEncontrado.Telefono = negocio.Telefono;
                negocioEncontrado.PorcentajeImpuesto = negocio.PorcentajeImpuesto;
                negocioEncontrado.SimboloMoneda = negocio.SimboloMoneda;
                negocioEncontrado.NombreLogo = negocioEncontrado.NombreLogo == "" ? nombreLogo : negocioEncontrado.NombreLogo;
                if(logo != null)
                {
                    string urlLogo = await _fireBaseServicio.subirStorage(logo, "carpeta_logo", negocioEncontrado.NombreLogo);
                    negocioEncontrado.UrlLogo = urlLogo;
                }

                await _repositorio.Editar(negocioEncontrado);

                return negocioEncontrado;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
