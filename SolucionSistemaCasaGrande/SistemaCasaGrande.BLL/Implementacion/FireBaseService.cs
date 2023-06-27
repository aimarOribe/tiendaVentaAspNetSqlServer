using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaCasaGrande.BLL.Interfaces;
using Firebase.Auth;
using Firebase.Storage;
using SistemaCasaGrande.Entity;
using SistemaCasaGrande.DAL.Interfaces;

namespace SistemaCasaGrande.BLL.Implementacion
{
    public class FireBaseService : IFireBaseService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        public FireBaseService (IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<string> subirStorage(Stream streamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string UrlImagen = "";

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad!, elementSelector: c => c.Valor!);
                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);
                var cancellation = new CancellationTokenSource();
                var task = new FirebaseStorage(
                        Config["ruta"],
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel = true
                        })
                    .Child(Config[CarpetaDestino])
                    .Child(NombreArchivo)
                    .PutAsync(streamArchivo, cancellation.Token);

                UrlImagen = await task;
            }
            catch (Exception)
            {
                UrlImagen = "";
                throw;
            }

            return UrlImagen;
        }
        public async Task<bool> eliminarStorage(string CarpetaDestino, string NombreArchivo)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad!, elementSelector: c => c.Valor!);
                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);
                var cancellation = new CancellationTokenSource();
                var task = new FirebaseStorage(
                        Config["ruta"],
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel = true
                        })
                    .Child(Config[CarpetaDestino])
                    .Child(NombreArchivo)
                    .DeleteAsync();

                await task;

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        
    }
}
