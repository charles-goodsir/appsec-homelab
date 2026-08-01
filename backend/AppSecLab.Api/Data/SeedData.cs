
using AppSecLab.Api.Models;
using System.Linq;

namespace AppSecLab.Api.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (!db.Users.Any())
        {
            db.Users.AddRange(
                new User { Username = "administrator", Password = "admin123" },
                new User { Username = "wiener", Password = "peter" }
            );

            db.Products.AddRange(
                new Product { Name = "Laptop Stand", Description = "Adjustable aluminium laptop stand." },
                new Product { Name = "Mechanical Keyboard", Description = "Hot-swappable mechanical keyboard." },
                new Product { Name = "Noise-Cancelling Headphones", Description = "Over-ear headphones with ANC." }
            );

            db.SaveChanges();
        }
    }
}