using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ERP;

[Index("CategoryGuid", Name = "IX_CategoryGuid")]
public partial class Category
{
    [Key]
    public int CategoryId { get; private set; }
    public Guid CategoryGuid { get; private set; } //= Guid.NewGuid();

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public Category()
    {
        CategoryGuid = Guid.NewGuid();
    }
}
