using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net;
using SistemaCasaGrande.BLL.Interfaces;
using SistemaCasaGrande.DAL.Interfaces;
using SistemaCasaGrande.Entity;

namespace SistemaCasaGrande.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IFireBaseService _fireBaseServicio;
        private readonly IUtilidadesService _utilidadesServicio;
        private readonly ICorreoService _correoServicio;

        public UsuarioService(
            IGenericRepository<Usuario> repositorio,
            IFireBaseService fireBaseService,
            IUtilidadesService utilidadesService,
            ICorreoService correoService)
        {
            _repositorio = repositorio;
            _fireBaseServicio = fireBaseService;
            _utilidadesServicio = utilidadesService;
            _correoServicio = correoService;
        }

        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repositorio.Consultar();
            return query.Include(r => r.IdRolNavigation).ToList();
        }
        public async Task<Usuario> Crear(Usuario usuario, Stream foto = null, string nombreFoto = null, string urlPlantillaCorreo = "")
        {
            Usuario usuarioExiste = await _repositorio.Obtener(u => u.Correo == usuario.Correo);
            
            if(usuarioExiste != null) 
                throw new TaskCanceledException("El correo ya existe");

            try
            {
                string claveGenerada = _utilidadesServicio.GenerarClave();
                usuario.Clave = _utilidadesServicio.ConvertirSha256(claveGenerada);
                
                usuario.NombreFoto = nombreFoto;

                if(foto != null)
                {
                    string urlFoto = await _fireBaseServicio.subirStorage(foto, "carpeta_usuario", nombreFoto);
                    usuario.UrlFoto = urlFoto;
                }
                
                Usuario ususarioCreado = await _repositorio.Crear(usuario);

                if(ususarioCreado.IdUsuario == 0)             
                    throw new TaskCanceledException("No se pudo crear el usuario");
                
                if(urlPlantillaCorreo != "")
                {
                    urlPlantillaCorreo = urlPlantillaCorreo.Replace("[correo]", ususarioCreado.Correo).Replace("[clave]", claveGenerada);
                    string htmlCorreo = "";
                    HttpWebRequest request = (HttpWebRequest) WebRequest.Create(urlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader streamReader = null;
                            if(response.CharacterSet == null)
                                streamReader = new StreamReader(dataStream);
                            else
                                streamReader = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            htmlCorreo = streamReader.ReadToEnd();
                            response.Close();
                            streamReader.Close();
                        }
                    }

                    if (htmlCorreo != "")
                        await _correoServicio.EnviarCorreo(ususarioCreado.Correo, "Cuenta Creada", htmlCorreo);
                    
                }

                IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == ususarioCreado.IdUsuario);
                ususarioCreado = query.Include(r => r.IdRolNavigation).First();

                return ususarioCreado;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Usuario> Editar(Usuario usuario, Stream foto = null, string nombreFoto = null)
        {
            Usuario usuarioExiste = await _repositorio.Obtener(u => u.Correo == usuario.Correo && u.IdUsuario != usuario.IdUsuario);

            if (usuarioExiste != null)
                throw new TaskCanceledException("El correo ya existe");

            try
            {
                IQueryable<Usuario> queryUsuario = await _repositorio.Consultar(u => u.IdUsuario == usuario.IdUsuario);
                Usuario usuarioEditar = queryUsuario.First();
                usuarioEditar.Nombre = usuario.Nombre;
                usuarioEditar.Correo = usuario.Correo;
                usuarioEditar.Telefono = usuario.Telefono;
                usuarioEditar.IdRol = usuario.IdRol;
                usuarioEditar.EsActivo = usuario.EsActivo;

                if (usuarioEditar.NombreFoto == "")
                    usuarioEditar.NombreFoto = nombreFoto;

                if(foto != null)
                {
                    string urlFoto = await _fireBaseServicio.subirStorage(foto, "carpeta_usuario", usuarioEditar.NombreFoto);
                    usuarioEditar.UrlFoto = urlFoto;
                }

                bool respuesta = await _repositorio.Editar(usuarioEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se puedo modificar el usuario");

                Usuario usuarioEditado = queryUsuario.Include(r => r.IdRolNavigation).First();

                return usuarioEditado;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario);
                if (usuarioEncontrado == null) throw new TaskCanceledException("El usuario no existe");

                string nombreFoto = usuarioEncontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(usuarioEncontrado);

                if (respuesta)
                    await _fireBaseServicio.eliminarStorage("carpeta_usuario", nombreFoto);

                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string claveEncriptada = _utilidadesServicio.ConvertirSha256(clave);
            Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.Correo.Equals(correo) && u.Clave.Equals(claveEncriptada));

            return usuarioEncontrado;
        }
        public async Task<Usuario> ObtenerPorId(int idUsuario)
        {
            IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == idUsuario);
            Usuario resultado = query.Include(r => r.IdRolNavigation).FirstOrDefault();

            return resultado;
        }
        public async Task<bool> GuardarPerfil(Usuario usuario)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == usuario.IdUsuario);
                if(usuarioEncontrado == null) throw new TaskCanceledException("Usuario no existe");
                
                usuarioEncontrado.Correo = usuario.Correo;
                usuarioEncontrado.Telefono = usuario.Telefono;

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);

                return respuesta;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> CambiarClave(int idUsuario, string claveActual, string claveNueva)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario);
                
                if(usuarioEncontrado == null) throw new TaskCanceledException("Usuario no existe");

                if (usuarioEncontrado.Clave != _utilidadesServicio.ConvertirSha256(claveActual)) throw new TaskCanceledException("La contraseña ingresada como actual no es la correcta");

                usuarioEncontrado.Clave = _utilidadesServicio.ConvertirSha256(claveNueva);

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);

                return respuesta;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> RestablecerClave(string correo, string urlPlantillaCorreo)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.Correo == correo);
                
                if (usuarioEncontrado == null) throw new TaskCanceledException("No encontramos ningun usuario asociado a ese correo");

                string claveGenerada = _utilidadesServicio.GenerarClave();
                usuarioEncontrado.Clave = _utilidadesServicio.ConvertirSha256(claveGenerada);

                urlPlantillaCorreo = urlPlantillaCorreo.Replace("[clave]", claveGenerada);
                string htmlCorreo = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader streamReader = null;
                        if (response.CharacterSet == null)
                            streamReader = new StreamReader(dataStream);
                        else
                            streamReader = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                        htmlCorreo = streamReader.ReadToEnd();
                        response.Close();
                        streamReader.Close();
                    }
                }

                bool correoEnviado = false;

                if (htmlCorreo != "")
                    correoEnviado = await _correoServicio.EnviarCorreo(correo, "Contraseña Restablecida", htmlCorreo);

                if (!correoEnviado)
                    throw new TaskCanceledException("Tenemos problemas. Por favor intentelo de nuevo mas tarde");

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);

                return respuesta;          
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
