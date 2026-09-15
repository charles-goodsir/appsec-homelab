
using AppSecLab.Api.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace AppSecLab.Api.Data;



public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (!db.Users.Any())
        {
            var hasher = new PasswordHasher<User>();
            var admin = new User { Username = "administrator" };
            admin.PasswordHash = hasher.HashPassword(admin, "admin123");
            var wiener = new User { Username = "wiener" };
            wiener.PasswordHash = hasher.HashPassword(wiener, "peter");
            db.Users.AddRange(
                admin,
                wiener
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