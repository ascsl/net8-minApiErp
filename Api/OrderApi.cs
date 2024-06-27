using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ERP.Data;
using ERP.Dtos;

namespace ERP.Api;

internal static class OrderApi
{
    public static RouteGroupBuilder MapOrderApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/erp")
            .WithTags("Order Api");

        // GET Orders paginados
        group.MapGet("/orders", async (AppDbContext db, int pageSize = 5, int page = 0) =>
        {
            var data = await db.Orders
                .OrderBy(s => s.OrderId)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Include(o => o.OrderDetails)
                .Select(x => new {
                    Id = x.OrderGuid,
                    Name = x.Name,
                    Address = x.Address,
                    City = x.City,
                    PostalCode = x.PostalCode,
                    Details = x.OrderDetails.Select(detail => new
                    {
                        DetailId = detail.OrderDetailGuid,
                        ProductId = detail.Product.ProductGuid,
                        ProductName = detail.Product.Title,
                        Count = detail.Count,
                        Price = detail.UnitPrice,
                    }).ToList()
                })
                .ToListAsync();
            return data.Any()
                ? Results.Ok(data)
                : Results.NotFound();
        })
        .WithOpenApi();


        // GET single orders
        group.MapGet("/orders/{guid}", async (Guid guid, AppDbContext db, IMapper mapper) =>
        {
            //var data = await db.Orders.FirstOrDefaultAsync(p => p.OrderGuid == guid);
            var data = await db.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(p => p.OrderGuid == guid);

            if (data == null)
            {
                return Results.NotFound();
            }

            var orderWithDetails = new
            {
                Id = data.OrderGuid,
                Name = data.Name,
                Address = data.Address,
                City = data.City,
                PostalCode = data.PostalCode,
                Details = data.OrderDetails.Select(detail => new
                {
                    DetailId = detail.OrderDetailGuid,
                    ProductId = detail.Product.ProductGuid,
                    ProductName = detail.Product.Title,
                    Count = detail.Count,
                    Price = detail.UnitPrice,
                }).ToList()
            };

            return orderWithDetails != null
                ? Results.Ok(mapper.Map<OrderDto>(orderWithDetails))
                : Results.NotFound();
        })
        .WithOpenApi();

        // CREATE order
        group.MapPost("/orders", async (OrderDto dataDto, AppDbContext db, IMapper mapper) =>
        {
            var data = mapper.Map<Order>(dataDto);
            db.Orders.Add(data);
            await db.SaveChangesAsync();
            return Results.Created($"/orders/{data.OrderGuid}", mapper.Map<OrderDto>(data));
        })
        .WithOpenApi();

        // CREATE order detail
        group.MapPost("/orders/detail", async (OrderDetailDto dataDto, AppDbContext db, IMapper mapper) =>
        {
            var data = mapper.Map<OrderDetail>(dataDto);
            db.OrderDetails.Add(data);
            await db.SaveChangesAsync();
            return Results.Created($"/orders/detail/{data.OrderDetailGuid}", mapper.Map<OrderDetailDto>(data));
        })
        .WithOpenApi();

        // UPDATE order
        group.MapPut("/orders/{guid}", async (Guid guid, OrderDto dataDto, AppDbContext db, IMapper mapper) =>
        {
            var data = await db.Orders.FindAsync(guid);
            if (data == null)
                return Results.NotFound();

            mapper.Map(dataDto, data);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        // UPDATE order detail
        group.MapPut("/orders/detail/{guid}", async (Guid guid, OrderDetailDto dataDto, AppDbContext db, IMapper mapper) =>
        {
            var data = await db.OrderDetails.FindAsync(guid);
            if (data == null)
                return Results.NotFound();

            mapper.Map(dataDto, data);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        // DELETE order
        group.MapDelete("/orders/{guid}", async (Guid guid, AppDbContext db) =>
        {
            var data = await db.Orders.FindAsync(guid);
            if (data == null)
                return Results.NotFound();

            db.Orders.Remove(data);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        // DELETE order detail
        group.MapDelete("/orders/detail/{guid}", async (Guid guid, AppDbContext db) =>
        {
            var data = await db.OrderDetails.FindAsync(guid);
            if (data == null)
                return Results.NotFound();

            db.OrderDetails.Remove(data);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithOpenApi();

        return group;
    }
}
