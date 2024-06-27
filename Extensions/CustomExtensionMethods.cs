using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using HealthChecks.UI.Client;
using ERP.Data;

namespace ERP.Extensions;

// Extensiones personalizadas del tipo IServiceCollection
public static class CustomExtensionMethods
{
    // Default Database
    public const string DefaultDB = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog = PartsUnlimitedWebsite; Integrated Security = true";

    public static IServiceCollection AddCustomSqlServerDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("dbconnection") ?? CustomExtensionMethods.DefaultDB;
        services.AddSqlServer<AppDbContext>(connectionString);

        return services;
    }

    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("dbconnection") ?? CustomExtensionMethods.DefaultDB;

        var hcBuilder = services.AddHealthChecks();
        hcBuilder
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddSqlServer(
                connectionString,
                name: "dbconnection-check",
                tags: new string[] { "dbconnection" });

        return services;
    }

    public static IServiceCollection AddCustomOpenApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Open API
        services.AddEndpointsApiExplorer();

        // Add framework services.
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "José Luis - ERP HTTP API - Curso .NET 8",
                Version = "v1",
                Description = "The ERP Microservice HTTP API. This is a Data-Driven/CRUD microservice"
            });
        });

        return services;
    }

    public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration, string policyName)
    {
        // Configure Open API
        services.AddCors(options =>
        {
            options.AddPolicy(policyName,
                policy =>
                {
                    policy.WithOrigins("https://*.tipesoft.com",
                                        "https://open-devlabs.com")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                });
        });

        return services;
    }
}

// Extensiones personalizadas del tipo IServiceCollection
public static class CustomMiddlewareExtensionMethods
{
    public static IEndpointRouteBuilder MapCustomHealthCheck(this IEndpointRouteBuilder routes, IConfiguration configuration)
    {
        routes.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        routes.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });

        return routes;
    }

    public static IEndpointRouteBuilder MapCustom(this IEndpointRouteBuilder routes, IConfiguration configuration)
    {

        return routes;
    }

    public static void DatabaseInit(this WebApplication app)
    {
        // TODO: Buscar mejor solución
        using (var scope = app.Services.CreateScope())
        {
            bool dbCreated = false;

            try
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbCreated = context.Database.EnsureCreated();
                //if (dbCreated) DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }

    }

    public static void ConfigureSwagger(this WebApplication app)
    {
        // TODO: Buscar mejor solución
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.Map("/", () => Results.Redirect("/swagger"));
        }
    }
}
