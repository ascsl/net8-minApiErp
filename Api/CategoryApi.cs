using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ERP.Data;
using ERP.Dtos;

namespace ERP.Api;

internal static class CategoryApi
{
    public static RouteGroupBuilder MapCategoryApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/erp")
            .WithTags("Category Api");


        // GET Categories paginados
        group.MapGet("/categories", async (AppDbContext db, int pageSize = 10, int page = 0) =>
        {
            var data = await db.Categories
                .OrderBy(s => s.CategoryGuid)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(x => new {
                    Id = x.CategoryGuid,
                    Name = x.Name,
                })
                .ToListAsync();
            return data.Any()
                ? Results.Ok(data)
                : Results.NotFound();
        })
        .WithOpenApi();

        // GET single Category
        group.MapGet("/categories/{guid}", async (Guid guid, AppDbContext db, IMapper mapper) =>
        {
            var data = await db.Categories.FirstOrDefaultAsync(p => p.CategoryGuid == guid);
            return data != null
                ? Results.Ok(mapper.Map<CategoryDto>(data))
                : Results.NotFound();
        })
        .WithOpenApi();

        // CREATE Category
        group.MapPost("/categories", async (CategoryDto dataDto, AppDbContext db, IMapper mapper) =>
        {
            var data = mapper.Map<Category>(dataDto);
            //category.CategoryGuid = Guid.NewGuid();
            db.Categories.Add(data);
            await db.SaveChangesAsync();
            return Results.Created($"/categories/{data.CategoryGuid}", mapper.Map<CategoryDto>(data));
        })
        .WithOpenApi();

        // UPDATE Category
        group.MapPut("/categories/{guid}", async (Guid guid, CategoryDto dataDto, AppDbContext db, IMapper mapper) =>
        {
            var data = await db.Categories.FindAsync(guid);
            if (data == null)
                return Results.NotFound();

            mapper.Map(dataDto, data);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        // DELETE Category
        group.MapDelete("/categories/{guid}", async (Guid guid, AppDbContext db) =>
        {
            var data = await db.Categories.FindAsync(guid);
            if (data == null)
                return Results.NotFound();

            db.Categories.Remove(data);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        return group;
    }
}
