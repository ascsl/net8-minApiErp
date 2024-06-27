using Microsoft.EntityFrameworkCore;
using MinimalAPIERP.Infraestructure.Automapper;
using ERP.Api;
using ERP.Data;
using ERP.Extensions;


// FASE 1
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery();

builder.Services
    .AddCustomSqlServerDb(builder.Configuration)
    .AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddCustomHealthCheck(builder.Configuration)
    .AddCustomOpenApi(builder.Configuration);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

// FASE 2
app.MapCustomHealthCheck(builder.Configuration);

app.UseAntiforgery();

app.DatabaseInit();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        // Manejo de excepciones
        Console.WriteLine(ex.ToString());
    }
}

app.ConfigureSwagger();

app.MapCategoryApi();
app.MapProductApi();
app.MapStoreApi();
app.MapRaincheckApi();
//app.MapCartItemApi();
app.MapOrderApi();
//app.MapOrderDetailApi();

app.Run();
