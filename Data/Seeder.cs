using ERP;
using ERP.Data;
using Microsoft.EntityFrameworkCore;

public static class DataSeeder
{
    public static void Seed(AppDbContext db)
    {
        // Verifica si la base de datos está vacía
        if (!db.Categories.Any())
        {
            db.Categories.AddRange(
                new Category { Name = "Categoria 1", },
                new Category { Name = "Categoria 2", }
            );
        }

        if (!db.Products.Any())
        {
            db.Products.AddRange(
                new Product { Title = "Producto 1", },
                new Product { Title = "Producto 2", }
            );
        }

        if (!db.Stores.Any())
        {
            // Si la base de datos está vacía, agrega algunos datos
            db.Stores.AddRange(
                new Store { Name = "Tienda 1", },
                new Store { Name = "Tienda 2", }
            );
        }

        // Guarda los cambios
        db.SaveChanges();
    }
}
