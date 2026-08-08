using DevCCSS.Web.Common;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using QuestPDF.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/web-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 15)
    .CreateLogger();

try
{
    Log.Information("Iniciando DevCCSS.Web...");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

builder.Services.AddMemoryCache();
builder.Services.AddScoped<PermisoAuthorizationFilter>();
builder.Services.AddControllersWithViews(options =>
{
    // Filtro global: conecta Roles_Permisos con la autorizacion real
    // (ver PermisoAuthorizationFilter). Se suma a los [Authorize(Roles=...)]
    // existentes, no los reemplaza.
    options.Filters.Add(typeof(PermisoAuthorizationFilter));
});
builder.Services.AddHttpContextAccessor();

// Clientes WCF (uno por servicio).  agrega aqui los suyos
builder.Services.AddScoped<SeguridadClient>();
builder.Services.AddScoped<PacienteClient>();
builder.Services.AddScoped<MedicamentoClient>();
builder.Services.AddScoped<InventarioClient>();
builder.Services.AddScoped<EnfermedadClient>();
builder.Services.AddScoped<VisitanteClient>();
builder.Services.AddScoped<EmpleadoClient>();
builder.Services.AddScoped<UsuarioClient>();
builder.Services.AddScoped<CitaClient>();
builder.Services.AddScoped<NacimientoClient>();
builder.Services.AddScoped<VentaClient>();
builder.Services.AddScoped<BitacoraClient>();
builder.Services.AddScoped<PermisoClient>();
builder.Services.AddScoped<ExamenMedicoClient>();
builder.Services.AddScoped<HabitacionClient>();
builder.Services.AddScoped<EspecialidadClient>();
builder.Services.AddScoped<MedicoClient>();

// Autenticacion por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSerilogRequestLogging();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "DevCCSS.Web se detuvo de forma inesperada.");
}
finally
{
    Log.CloseAndFlush();
}
