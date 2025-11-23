using Microsoft.EntityFrameworkCore;
using Store.G02.Domain.Contracts;
using Store.G02.Domain.Entities.Products;
using Store.G02.Persistence.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Store.G02.Persistence
{
    public class DbInitializer : IDbInitializer
    {
        private readonly StoreDbContext _context;

        public DbInitializer(StoreDbContext context)
        {
            _context = context;
        }

        public async Task InitializerAsync()
        {

            // Create the database if it does not exist
            // Update the database to the latest version
            var PendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            if (PendingMigrations.Any())
            {
                await _context.Database.MigrateAsync();
            }


            // Seed initial data if necessary
            if (!_context.ProductBrands.Any())
            {
                var BrandsData = await File.ReadAllTextAsync(@"..\Infrastructure\Store.G02.Persistence\Data\DataSeeding\brands.json");
                var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandsData);
                if (Brands != null && Brands.Count > 0)
                {
                    await _context.ProductBrands.AddRangeAsync(Brands);
                }
            }

            if (!_context.ProductTypes.Any())
            {
                var TypesData = await File.ReadAllTextAsync(@"..\Infrastructure\Store.G02.Persistence\Data\DataSeeding\types.json");
                var Types = JsonSerializer.Deserialize<List<ProductType>>(TypesData);
                if (Types != null && Types.Count > 0)
                {
                    await _context.ProductTypes.AddRangeAsync(Types);
                }
            }

            if (!_context.Products.Any())
            {
                var ProductsData = await File.ReadAllTextAsync(@"..\Infrastructure\Store.G02.Persistence\Data\DataSeeding\products.json");
                var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);
                if (Products != null && Products.Count > 0)
                {
                    await _context.Products.AddRangeAsync(Products);
                }
            }

            await _context.SaveChangesAsync();

        }
    }
}
