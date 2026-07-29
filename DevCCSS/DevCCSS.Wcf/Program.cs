using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using DevCCSS.Wcf.Models;
using DevCCSS.Wcf.Services;

var builder = WebApplication.CreateBuilder(args);

// Servicios de CoreWCF (el intermediario)
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

// Aqui registramos cada servicio del hospital.
// Cuando un companero termine su servicio, lo agrega aqui igual que estos.
builder.Services.AddTransient<SeguridadService>();
builder.Services.AddTransient<PacienteService>();
builder.Services.AddTransient<MedicamentoService>();
builder.Services.AddTransient<InventarioService>();
builder.Services.AddTransient<EnfermedadService>();
builder.Services.AddTransient<VisitanteService>();
builder.Services.AddTransient<EmpleadoService>();
builder.Services.AddTransient<UsuarioService>();
builder.Services.AddTransient<CitaService>();
builder.Services.AddScoped<CitaRepository>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddTransient<NacimientoService>();
builder.Services.AddTransient<VentaService>();
builder.Services.AddTransient<BitacoraService>();
builder.Services.AddTransient<PermisoService>();
//  agregar aqui EmpleadoService, CitaService, etc.

var app = builder.Build();

// Cuota de mensaje elevada (por defecto BasicHttpBinding es de solo 64KB,
// muy poco para listados grandes como la bitacora de auditoria).
var binding = new BasicHttpBinding
{
    MaxReceivedMessageSize = 10 * 1024 * 1024,
    MaxBufferSize = 10 * 1024 * 1024,
    ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
};

app.UseServiceModel(serviceBuilder =>
{
    // Seguridad / Login
    serviceBuilder.AddService<SeguridadService>();
    serviceBuilder.AddServiceEndpoint<SeguridadService, ISeguridadService>(
        binding,
        "/SeguridadService.svc");

    // Pacientes modulo
    serviceBuilder.AddService<PacienteService>();
    serviceBuilder.AddServiceEndpoint<PacienteService, IPacienteService>(
        binding,
        "/PacienteService.svc");

    // Medicamento modulo
    serviceBuilder.AddService<MedicamentoService>();
    serviceBuilder.AddServiceEndpoint<MedicamentoService, IMedicamentoService>(
        binding,
        "/MedicamentoService.svc");

    serviceBuilder.AddService<InventarioService>();
    serviceBuilder.AddServiceEndpoint<InventarioService, IInventarioService>(
        binding,
        "/InventarioService.svc");

    serviceBuilder.AddService<EnfermedadService>();
    serviceBuilder.AddServiceEndpoint<EnfermedadService, IEnfermedadService>(
        binding,
        "/EnfermedadService.svc");

    serviceBuilder.AddService<VisitanteService>();
    serviceBuilder.AddServiceEndpoint<VisitanteService, IVisitanteService>(
        binding,
        "/VisitanteService.svc");

    serviceBuilder.AddService<EmpleadoService>();
    serviceBuilder.AddServiceEndpoint<EmpleadoService, IEmpleadoService>(
        binding,
        "/EmpleadoService.svc");

    serviceBuilder.AddService<UsuarioService>();
    serviceBuilder.AddServiceEndpoint<UsuarioService, IUsuarioService>(
        binding,
        "/UsuarioService.svc");

    serviceBuilder.AddService<CitaService>();
    serviceBuilder.AddServiceEndpoint<CitaService, ICitaService>(
        binding,
        "/CitaService.svc");

    serviceBuilder.AddService<NacimientoService>();
    serviceBuilder.AddServiceEndpoint<NacimientoService, INacimientoService>(
        binding,
        "/NacimientoService.svc");

    serviceBuilder.AddService<VentaService>();
    serviceBuilder.AddServiceEndpoint<VentaService, IVentaService>(
        binding,
        "/VentaService.svc");

    serviceBuilder.AddService<BitacoraService>();
    serviceBuilder.AddServiceEndpoint<BitacoraService, IBitacoraService>(
        binding,
        "/BitacoraService.svc");

    serviceBuilder.AddService<PermisoService>();
    serviceBuilder.AddServiceEndpoint<PermisoService, IPermisoService>(
        binding,
        "/PermisoService.svc");

    //  cada modulo agrega su AddService + AddServiceEndpoint aqui.
});

// Habilitar el WSDL (?wsdl) para poder ver/probar el servicio
var metadata = app.Services.GetRequiredService<ServiceMetadataBehavior>();
metadata.HttpGetEnabled = true;

app.Run();
