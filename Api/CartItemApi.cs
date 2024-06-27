using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoMapper;
using ERP.Data;
using ERP.Dtos;

namespace ERP.Api;

internal static class CartItemApi
{
    public static RouteGroupBuilder MapCartItemApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/erp")
            .WithTags("CartItem Api");

        group.MapGet("/user", (ClaimsPrincipal user) =>
            Results.Ok(user.Identity))
        .WithOpenApi();

        // GET CartItems paginados
        group.MapGet("/cart", async (AppDbContext db, int pageSize = 10, int page = 0) =>
        {
            var data = await db.CartItems
                .OrderBy(s => s.CartItemGuid)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(x => new {
                    Id = x.CartItemGuid,
                    ProductId = x.Product.ProductGuid,
                    ProductName = x.Product.Title,
                    CategoryId = x.Product.Category.CategoryGuid,
                })
                .ToListAsync();

            return data.Any()
                ? Results.Ok(data)
                : Results.NotFound();
        })
        .WithOpenApi();

        // GET CartItem by Guid
        group.MapGet("/cart/{guid}", async Task<Results<Ok<CartItemDto>, NotFound>> (Guid guid, AppDbContext db, IMapper mapper) =>
        {
            var data = await db.CartItems.FirstOrDefaultAsync(m => m.CartItemGuid == guid);
            return data != null
                ? TypedResults.Ok(mapper.Map<CartItemDto>(data))
                : TypedResults.NotFound();
        })
        .WithOpenApi();


        // CREATE CartItem
        group.MapPost("/cart", async (CartItemDto dataDto, AppDbContext db, IMapper mapper) =>
        {
            var data = mapper.Map<CartItem>(dataDto);
            //data.CartItemGuid = Guid.NewGuid();
            db.CartItems.Add(data);
            await db.SaveChangesAsync();
            return Results.Created($"/cart/{data.CartItemGuid}", mapper.Map<CartItemDto>(data));
        })
        .WithOpenApi();

        // UPDATE CartItem
        group.MapPut("/cart/{guid}", async (Guid guid, CartItemDto dataDto, AppDbContext db, IMapper mapper) =>
        {
            var data = await db.CartItems.FindAsync(guid);
            if (data == null)
                return Results.NotFound();

            mapper.Map(dataDto, data);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        // DELETE CartItem
        group.MapDelete("/cart/{guid}", async (Guid guid, AppDbContext db) =>
        {
            var data = await db.CartItems.FindAsync(guid);
            if (data == null)
                return Results.NotFound();

            db.CartItems.Remove(data);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        return group;
    }
}

