using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoMapper;
using ERP.Data;
using ERP.Dtos;

namespace ERP.Api;

internal static class StoreApi
{
    public static RouteGroupBuilder MapStoreApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/erp")
            .WithTags("Store Api");

        group.MapGet("/user", (ClaimsPrincipal user) =>
            Results.Ok(user.Identity))
        .WithOpenApi();

        group.MapGet("/store/{guid}", async Task<Results<Ok<StoreDto>, NotFound>> (Guid guid, AppDbContext db, IMapper mapper) =>
        {
            var data = await db.Stores.FirstOrDefaultAsync(m => m.StoreGuid == guid);
            return data != null
                ? TypedResults.Ok(mapper.Map<StoreDto>(data))
                : TypedResults.NotFound();
        })
        .WithOpenApi();

        // GET Stores paginados
        group.MapGet("/stores", async (AppDbContext db, int pageSize = 10, int page = 0) =>
        {
            var data = await db.Stores
                .OrderBy(s => s.StoreGuid)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(x => new {
                    Id = x.StoreGuid,
                    Name = x.Name,
                })
                .ToListAsync();

            return data.Any()
                ? Results.Ok(data)
                : Results.NotFound();
        })
        .WithOpenApi();

        // CREATE Product
        group.MapPost("/store", async (StoreDto dataDto, AppDbContext db, IMapper mapper) =>
        {
            var data = mapper.Map<Store>(dataDto);
            //store.StoreGuid = Guid.NewGuid();
            db.Stores.Add(data);
            await db.SaveChangesAsync();
            return Results.Created($"/products/{data.StoreGuid}", mapper.Map<StoreDto>(data));
        })
        .WithOpenApi();

        // UPDATE Store
        group.MapPut("/store/{guid}", async (Guid guid, StoreDto dataDto, AppDbContext db, IMapper mapper) =>
        {
            var data = await db.Stores.FindAsync(guid);
            if (data == null)
                return Results.NotFound();

            mapper.Map(dataDto, data);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        // DELETE Store
        group.MapDelete("/store/{guid}", async (Guid guid, AppDbContext db) =>
        {
            var data = await db.Stores.FindAsync(guid);
            if (data == null)
                return Results.NotFound();

            db.Stores.Remove(data);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        return group;
    }
}

/*
using AutoMapper;
using ERP.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MinimalAPIERP.Dtos;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ERP.Api;

internal static class StoreApi
{
    public static RouteGroupBuilder MapStoreApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/erp")
            .WithTags("Store Api");

        group.MapGet("/user", (ClaimsPrincipal user) =>
        {
            return user.Identity;

        })
        .WithOpenApi();

        group.MapGet("/store/{storeid}", async Task<Results<Ok<StoreDto>, NotFound>> (int storeid, AppDbContext db, IMapper mapper) => {
            return mapper.Map<StoreDto>(await db.Stores.FirstOrDefaultAsync(m => m.StoreId == storeid)) 
                is StoreDto store
                    ? TypedResults.Ok(store)
                    : TypedResults.NotFound();
        })
        .WithOpenApi();


        group.MapGet("/storea", async Task<Results<Ok<IList<Store>>, NotFound>> (AppDbContext db) =>
            await db.Stores.ToListAsync()
                is IList<Store> stores
                    ? TypedResults.Ok(stores)
                    : TypedResults.NotFound())
            .WithOpenApi();


        group.MapGet("/storeb", async Task<Results<Ok<IList<Store>>, NotFound>> (AppDbContext db, int pageSize = 10, int page = 0) =>
            await db.Stores.Skip(page * pageSize).Take(pageSize).ToListAsync()
                is IList<Store> stores
                    ? TypedResults.Ok(stores)
                    : TypedResults.NotFound())
            .WithOpenApi();

        group.MapGet("/storec1", async Task<Results<Ok<IList<Store>>, NotFound>> (AppDbContext db, int pageSize = 10, int page = 0) =>
            await db.Stores
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(store => new { store.StoreId, store.Name })
            .ToListAsync()
                is IList<Store> stores
                    ? TypedResults.Ok(stores)
                    : TypedResults.NotFound())
            .WithOpenApi();

        group.MapGet("/storec2", async  (AppDbContext db, int pageSize = 10, int page = 0) =>
        {
            var data = await db.Stores
                .Skip(page * pageSize)
                .Take(pageSize)
                .Include(s => s.Rainchecks)
                .Select(store => new { store.StoreId, store.Name })
                .ToListAsync();

            return data.Any()
                ? Results.Ok(data)
                : Results.NotFound();
        })
        .WithOpenApi();

        // TODO: Mover a config
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web) {
            //PropertyNameCaseInsensitive = false,
            //PropertyNamingPolicy = null,
            WriteIndented = true,
            //IncludeFields = false,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            //ReferenceHandler = ReferenceHandler.Preserve
        };

        group.MapGet("/stored", async Task<Results<Ok<IList<Store>>, NotFound>> (AppDbContext db) =>
            await db.Stores.Include(s => s.Rainchecks).ToListAsync()
                is IList<Store> stores
                    ? TypedResults.Ok(stores)
                    : TypedResults.NotFound())
            .WithOpenApi();

        group.MapGet("/storee", async (AppDbContext db) =>
            await db.Stores.Include(s => s.Rainchecks).ToListAsync()
                is IList<Store> stores
                    ? Results.Json(stores, options)
                    : Results.NotFound())
            .WithOpenApi();

        group.MapGet("/storef", async (AppDbContext db) =>
            await db.Stores.Include(s => s.Rainchecks).ToListAsync()
                is IList<Store> stores
                    ? Results.Json(stores, options)
                    : Results.NotFound())
            .WithOpenApi();

        return group;
    }
}
*/
