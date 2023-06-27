using SistemaCasaGrande.IOC;
using SistemaCasaGrande.AplicacionWeb.Utilidades.AutoMapper;
using SistemaCasaGrande.AplicacionWeb.Utilidades.Extensiones;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt => {
    opt.LoginPath = "/Acceso/Login";
    opt.ExpireTimeSpan = TimeSpan.FromMinutes(20);
});

builder.Services.InyectarDependencia(builder.Configuration);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "Utilidades/LibreriaPDF/libwkhtmltox.dll"));
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();
