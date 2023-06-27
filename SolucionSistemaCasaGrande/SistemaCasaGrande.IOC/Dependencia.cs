using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaCasaGrande.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using SistemaCasaGrande.DAL.Implementacion;
using SistemaCasaGrande.DAL.Interfaces;
using SistemaCasaGrande.BLL.Interfaces;
using SistemaCasaGrande.BLL.Implementacion;

namespace SistemaCasaGrande.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DbcasagrandeContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("cadenaSQL"));
            });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            
            services.AddScoped<IVentaRepository, VentaRepository>();
            services.AddScoped<ICorreoService, CorreoService>();
            services.AddScoped<IFireBaseService, FireBaseService>();
            services.AddScoped<IUtilidadesService, UtilidadesService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<INegocioService, NegocioService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<ITipoDocumentoVentaService, TipoDocumentoVentaService>();
            services.AddScoped<IVentaService, VentaService>();
            services.AddScoped<IDashBoardService, DashBoardService>();
            services.AddScoped<IMenuService, MenuService>();
        }
    }
}