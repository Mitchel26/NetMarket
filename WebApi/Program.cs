using BusinessLogic.Data;
using BusinessLogic.Logic;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using WebApi.DTOs;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

// MarketDbContext: Contine las tablas de transacciones
var connectionString1 = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MarketDbContext>(opciones =>
{
    opciones.UseSqlServer(connectionString1);
});

// SeguridadDbContext: Encargado de contener las tablas de Identity
var connectionString2 = builder.Configuration.GetConnectionString("IdentitySeguridad");
builder.Services.AddDbContext<SeguridadDbContext>(opciones =>
{
    opciones.UseSqlServer(connectionString2);
});
// Configuración del IdentityUser(Usuario)  y IdentityRole
builder.Services.AddIdentity<Usuario, IdentityRole>()
                .AddEntityFrameworkStores<SeguridadDbContext>()
                .AddDefaultTokenProviders()
                .AddRoles<IdentityRole>();

builder.Services.TryAddSingleton<ISystemClock, SystemClock>(); // Permite agregar la hh:mm:ss cuando se inserte un registro el las entidades de seguridad

// Conexión de Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(c =>
{
    var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
    return ConnectionMultiplexer.Connect(configuration);
});


// Autenticación y acceso al Key y Issuer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"])),
        ValidIssuer = builder.Configuration["Token:Issuer"],
        ValidateIssuer = true,
        ValidateAudience = false
    };
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>(); // Repositorio del Token
builder.Services.AddTransient<IProductoRepository, ProductoRepository>(); // Repositorio Producto
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>)); // Repositorio Genérico 
builder.Services.AddAutoMapper(typeof(MappingProfiles)); // Uso de AutoMapper
builder.Services.AddTransient<ICarritoCompraRepository, CarritoCompraRepository>(); // Respositorio CarritoCompra
builder.Services.AddTransient(typeof(IGenericSeguridadRepository<>), typeof(GenericSeguridadRepository<>)); // Repositorio Genérico clases IdentityUser SeguridadDbContext

builder.Services.AddScoped<IOrdenCompraService, OrdenCompraService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsRule", rule =>
    {
        rule.AllowAnyHeader().AllowAnyMethod().WithOrigins("*");
    });
});

var app = builder.Build();

// Aplicar las migraciones de manera automática(Cuando se ejecuta la solución)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerfactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        // Migración Automática del MarketDbContext
        var contexto = services.GetRequiredService<MarketDbContext>();
        await contexto.Database.MigrateAsync();
        await MarketDbContextData.CargarDataAsync(contexto, loggerfactory);

        // Migración Automática del SeguridadDbContext
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var identityContext = services.GetRequiredService<SeguridadDbContext>();
        await identityContext.Database.MigrateAsync();
        await SeguridadDbContextData.SeedUserAsync(userManager, roleManager);

    }
    catch (Exception e)
    {
        var logger = loggerfactory.CreateLogger<Program>();
        logger.LogError(e, "Errores en el proceso de migración");
    }
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/errors", "?code={0}");  // Ejecuta en caso de error mi ErrorController

app.UseRouting();

app.UseCors("CorsRule");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
