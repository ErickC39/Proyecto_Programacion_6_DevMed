using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

builder.Services.AddControllersWithViews();

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

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
