using System.ComponentModel.DataAnnotations;

namespace ERP.Dtos;

public class CategoryDto
{
    public Guid CategoryGuid { get; set; }
    public string? Name { get; set; }
}

public class ProductDto
{
    public Guid ProductGuid { get; set; }
    public string? Name { get; set; }
    public CategoryDto? Category { get; set; }
}

public class StoreDto
{
    public Guid StoreGuid { get; set; }
    public string? Name { get; set; }
}

public class RaincheckDto
{
    public Guid RaincheckGuid { get; set; }
    public string? Name { get; set; }
    public int Count { get; set; }
    public double SalePrice { get; set; }
    public StoreDto? Store { get; set; }
    public ProductDto? Product { get; set; }
}

public class OrderDto
{
    public Guid OrderGuid { get; set; }
    public DateTime OrderDate { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public virtual ICollection<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
}

public class OrderDetailDto
{
    public Guid OrderDetailGuid { get; set; }
    public ProductDto? Product { get; set; }
    public int Count { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CartItemDto
{
    public Guid CartItemGuid { get; set; }
    public ProductDto? Product { get; set; }
    public int Count { get; set; }
}
