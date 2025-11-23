using Microsoft.EntityFrameworkCore;
using Store.G02.Services;
using Store.G02.Domain.Contracts;
using Store.G02.Persistence;
using Store.G02.Persistence.Data.Contexts;
using Store.G02.Services.Abstractions;
using Store.G02.Services.Mapping.Products;
using Store.G02.Web.Middleware;
using Microsoft.AspNetCore.Mvc;
using Store.G02.Shared.ErrorModels;
using StackExchange.Redis;
using Store.G02.Services.Mapping.Baskets;
using Store.G02.Persistence.Repositories;

namespace Store.G02.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IServiceManager, ServiceManager>();

            builder.Services.AddAutoMapper(Config =>
            {
                Config.AddProfile(new ProductProfile(builder.Configuration));
                Config.AddProfile(new BasketProfile());
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(IServiceProvider =>
                ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection"))
            );

            builder.Services.AddScoped<IBasketRepository, BasketRepository>();
            builder.Services.AddScoped<ICacheRepository, CacheRepository>();


            #region Understand Thing 
            // https://chatgpt.com/share/691ff64a-3280-800f-99b3-a868c7c4ab99
            #endregion

            builder.Services.Configure<ApiBehaviorOptions>(config =>
            {
                config.InvalidModelStateResponseFactory = (ActionContext) =>
                {
                    var errors = ActionContext.ModelState.Where(Field => Field.Value.Errors.Any())
                                                         .Select(Field => new ValidationError()
                                                         {
                                                             Field = Field.Key,
                                                             Errors = Field.Value.Errors.Select(Error => Error.ErrorMessage)
                                                         }).ToList();
                    var ResponseBody = new ValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(ResponseBody);
                };                                       
            });

            var app = builder.Build();


            using var ScopedServices = app.Services.CreateScope();
            var DbInitializer = ScopedServices.ServiceProvider.GetRequiredService<IDbInitializer>();
            await DbInitializer.InitializerAsync();

            app.UseMiddleware<GlobalErrorHandlingMiddleware>();

            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
