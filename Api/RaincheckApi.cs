using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using ERP.Data;
using ERP.Dtos;

namespace ERP.Api;

internal static class RaincheckApi
{
    public static RouteGroupBuilder MapRaincheckApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/erp")
            .WithTags("Raincheck Api");

        // TODO: Mover a config
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            //PropertyNameCaseInsensitive = false,
            //PropertyNamingPolicy = null,
            WriteIndented = true,
            //IncludeFields = false,
            MaxDepth = 0,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            //ReferenceHandler = ReferenceHandler.Preserve
            //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        // GET Rainchecks paginados
        group.MapGet("/rainchecks", async (AppDbContext db, int pageSize = 10, int page = 0) =>
        {
            var data = await db.Rainchecks
                .OrderBy(s => s.RaincheckGuid)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Include(s => s.Product)
                .Include(s => s.Store)
                .Select(x => new {
                    Id = x.RaincheckGuid,
                    ProductId = x.Product.ProductGuid,
                    CategoryId = x.Product.Category.CategoryGuid,
                    CategoryName = x.Product.Category.Name,
                    StoreId = x.Store.StoreGuid,
                    SalePrice = x.SalePrice,
                    Count = x.Count,
                })
                .ToListAsync();
            return data.Any()
                ? Results.Ok(data)
                : Results.NotFound();
        })
        .WithOpenApi();

        // GET single Raincheck
        group.MapGet("/rainchecks/{guid}", async (Guid guid, AppDbContext db, IMapper mapper) =>
        {
            var data = await db.Rainchecks.FirstOrDefaultAsync(p => p.RaincheckGuid == guid);
            return data != null
                ? Results.Ok(mapper.Map<RaincheckDto>(data))
                : Results.NotFound();
        })
        .WithOpenApi();

        // GET Rainchecks of single Product
        group.MapGet("/rainchecks/product/{guid}", async (Guid guid, AppDbContext db, int pageSize = 10, int page = 0) =>
        {
            var data = await db.Rainchecks
                .OrderBy(s => s.RaincheckGuid)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Include(s => s.Product)
                .Select(x => new {
                    Id = x.RaincheckGuid,
//                    ProductId = x.Product.ProductGuid,
//                    CategoryId = x.Product.Category.CategoryGuid,
//                    CategoryName = x.Product.Category.Name,
                    StoreId = x.Store.StoreGuid,
                    SalePrice = x.SalePrice,
                    Count = x.Count,
                })
                .ToListAsync();
            return data.Any()
                ? Results.Ok(data)
                : Results.NotFound();
        })
        .WithOpenApi();

        // GET Rainchecks of single Store
        group.MapGet("/rainchecks/store/{guid}", async (Guid guid, AppDbContext db, int pageSize = 10, int page = 0) =>
        {
            var data = await db.Rainchecks
                .OrderBy(s => s.RaincheckGuid)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Include(s => s.Store)
                .Select(x => new {
                    Id = x.RaincheckGuid,
                    ProductId = x.Product.ProductGuid,
                    CategoryId = x.Product.Category.CategoryGuid,
                    CategoryName = x.Product.Category.Name,
//                    StoreId = x.Store.StoreGuid,
                    SalePrice = x.SalePrice,
                    Count = x.Count,
                })
                .ToListAsync();
            return data.Any()
                ? Results.Ok(data)
                : Results.NotFound();
        })
        .WithOpenApi();

        // POST single Raincheck : No se crea un metodo POST ya que el alta deberia crearlo la base de datos

        // PUT single Raincheck : No se crea un metodo PUT ya que la modificacion deberia hacerla la base de datos al vender

        // DEL single Raincheck : No se crea un metodo DEL ya que desde la api no se deberia permitir el borrado

        return group;
    }
}

/*
group.MapGet("/rainchecksa", async Task<Results<Ok<IList<Raincheck>>, NotFound>> (AppDbContext db) =>
    await db.Rainchecks
        .Include(s => s.Product)
        .Include(s => s.Store)
        .ToListAsync()
            is IList<Raincheck> rainchecks
                ? TypedResults.Ok(rainchecks)
                : TypedResults.NotFound())
.WithOpenApi();

group.MapGet("/rainchecksb", async (AppDbContext db, int pageSize = 10, int page = 0) =>
{
    var data = await db.Rainchecks
        .OrderBy(s => s.RaincheckId)
        .Skip(page * pageSize)
        .Take(pageSize)
        .Include(s => s.Product)
            .ThenInclude(s => s.Category)
        .Include(s => s.Store)
        .Select(r => new { r.StoreId, r.Name, r.Product, r.Store })
        .ToListAsync();

return data.Any()
        ? Results.Json(data, options)
        : Results.NotFound();
})
.WithOpenApi();

group.MapGet("/rainchecksc", async (AppDbContext db, int pageSize = 10, int page = 0) =>
{
    var data = await db.Rainchecks
        .OrderBy(s => s.RaincheckId)
        .Skip(page * pageSize)
        .Take(pageSize)
        .Include(s => s.Product)
        .Include(s => s.Product.Category)
        .Include(s => s.Store)
        .Select(x => new {
            Name = x.Name,
            Count = x.Count,
            SalePrice = x.SalePrice,
            Store = new {
                Name = x.Store.Name,
            },
            Product = new { 
                Name = x.Product.Title, 
                Category = new {
                    Name = x.Product.Category.Name
                }
            }
        })
        .ToListAsync();

    return data.Any()
        ? Results.Json(data, options)
        : Results.NotFound();
})
.WithOpenApi();

group.MapGet("/rainchecksd", async Task<Results<Ok<List<RaincheckDto>>, NotFound>> (AppDbContext db, int pageSize = 10, int page = 0) =>
{
    var data = await db.Rainchecks
        .OrderBy(s => s.RaincheckId)
        .Skip(page * pageSize)
        .Take(pageSize)
        .Include(s => s.Product)
        .Include(s => s.Product.Category)
        .Include(s => s.Store)
        .Select(x => new RaincheckDto
        {
            Name = x.Name,
            Count = x.Count,
            SalePrice = x.SalePrice,
            Store = new StoreDto
            {
                Name = x.Store.Name
            },
            Product = new ProductDto
            {
                Name = x.Product.Title,
                Category = new CategoryDto
                {
                    Name = x.Product.Category.Name
                }
            }
        })
        .ToListAsync();

    return data.Any()
        ? TypedResults.Ok(data)
        : TypedResults.NotFound();

})
.WithOpenApi();
*/
