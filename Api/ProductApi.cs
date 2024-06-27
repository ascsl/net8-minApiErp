using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ERP.Data;
using ERP.Dtos;

namespace ERP.Api;

internal static class ProductApi
{
    public static RouteGroupBuilder MapProductApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/erp")
            .WithTags("Product Api");

        // GET Products paginados
        group.MapGet("/products", async (AppDbContext db, int pageSize = 10, int page = 0) =>
        {
            var data = await db.Products
                .OrderBy(s => s.ProductGuid)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Include(s => s.Category)
                .Select(x => new {
                    Id = x.ProductGuid,
                    Name = x.Title,
                    SalePrice = x.SalePrice,
                    CategoryId = x.Category.CategoryGuid,
                    CategoryName = x.Category.Name,
                })
                .ToListAsync();
            return data.Any()
                ? Results.Ok(data)
                : Results.NotFound();
        })
        .WithOpenApi();

        // GET single Product
        group.MapGet("/products/{guid}", async (Guid guid, AppDbContext db, IMapper mapper) =>
        {
            var data = await db.Products.FirstOrDefaultAsync(p => p.ProductGuid == guid);
            return data != null
                ? Results.Ok(mapper.Map<ProductDto>(data))
                : Results.NotFound();
        })
        .WithOpenApi();

        // CREATE Product
        group.MapPost("/products", async (ProductDto dataDto, AppDbContext db, IMapper mapper) =>
        {
            var data = mapper.Map<Product>(dataDto);
            //product.ProductGuid = Guid.NewGuid();
            db.Products.Add(data);
            await db.SaveChangesAsync();
            return Results.Created($"/products/{data.ProductGuid}", mapper.Map<ProductDto>(data));
        })
        .WithOpenApi();

        // UPDATE Product
        group.MapPut("/products/{guid}", async (Guid guid, ProductDto dataDto, AppDbContext db, IMapper mapper) =>
        {
            var data = await db.Products.FindAsync(guid);
            if (data == null)
                return Results.NotFound();

            mapper.Map(dataDto, data);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        // DELETE Product
        group.MapDelete("/products/{guid}", async (Guid guid, AppDbContext db) =>
        {
            var data = await db.Products.FindAsync(guid);
            if (data == null)
                return Results.NotFound();

            db.Products.Remove(data);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        return group;
    }
}
