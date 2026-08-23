using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.G02.Domain.Contracts;
using Store.G02.Domain.Entities.Identity;
using Store.G02.Domain.Entities.Orders;
using Store.G02.Domain.Entities.Products;
using Store.G02.Persistence.Data.Contexts;
using Store.G02.Persistence.Identity.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// https://gemini.google.com/share/4779da325294

namespace Store.G02.Persistence
{
    public class DbInitializer(StoreDbContext _context,
                               IdentityStoreDbContext _identityContext,
                               UserManager<AppUser> _userManager,
                               RoleManager<IdentityRole> _roleManager)
                               : IDbInitializer
    {
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


            if (!_context.DeliveryMethods.Any())
            {
                var DeliveryMethodsData = await File.ReadAllTextAsync(@"..\Infrastructure\Store.G02.Persistence\Data\DataSeeding\delivery.json");
                var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);
                if (DeliveryMethods != null && DeliveryMethods.Count > 0)
                {
                    await _context.DeliveryMethods.AddRangeAsync(DeliveryMethods);
                }
            }

            await _context.SaveChangesAsync();

        }

        public async Task InitializerIdentityAsync()
        {
            var PendingMigrations = await _identityContext.Database.GetPendingMigrationsAsync();
            if (PendingMigrations.Any())
            {
                await _identityContext.Database.MigrateAsync();
            }

            if (!_identityContext.Roles.Any())
            {
                var SuperAdminRole = new IdentityRole("SuperAdmin");
                var AdminRole = new IdentityRole("Admin");

                await _roleManager.CreateAsync(SuperAdminRole);
                await _roleManager.CreateAsync(AdminRole);
            }

            if (!_identityContext.Users.Any())
            {
                var SuperAdminUser = new AppUser()
                {
                    UserName = "SuperAdmin",
                    Email = "SuperAdmin@gmail.com",
                    DisplayName = "SuperAdmin",
                    PhoneNumber = "01111111111"
                };
                var AdminUser = new AppUser()
                {
                    UserName = "Admin",
                    Email = "Admin@gmail.com",
                    DisplayName = "Admin",
                    PhoneNumber = "01222222222"
                };
                await _userManager.CreateAsync(SuperAdminUser, "P@ssW0rd");
                await _userManager.CreateAsync(AdminUser, "P@ssW0rd");

                await _userManager.AddToRoleAsync(SuperAdminUser, "SuperAdmin");
                await _userManager.AddToRoleAsync(AdminUser, "Admin");

            }


        }
    }
}
