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
using Store.G02.Persistence.Identity.Contexts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Store.G02.Domain.Entities.Identity;
using Store.G02.Shared;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Store.G02.Services.Mapping.Orders;
using Store.G02.Services.Mapping.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Store.G02.Services.MailKitFeature;

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

            builder.Services.AddDbContext<IdentityStoreDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IServiceManager, ServiceManager>();

            builder.Services.AddAutoMapper(Config =>
            {
                Config.AddProfile(new ProductProfile(builder.Configuration));
                Config.AddProfile(new BasketProfile());
                Config.AddProfile(new OrderProfile());
                Config.AddProfile(new AuthProfile());
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(IServiceProvider =>
                ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection"))
            );

            builder.Services.AddScoped<IBasketRepository, BasketRepository>();
            builder.Services.AddScoped<ICacheRepository, CacheRepository>();


            builder.Services.AddIdentity<AppUser, IdentityRole>(identityOption =>
            {
                identityOption.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityStoreDbContext>()
            .AddDefaultTokenProviders();


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

            builder.Services.Configure<MailKitSetting>(builder.Configuration.GetSection("MailKitSetting"));
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWTOptions"));

            var JWTOptions = builder.Configuration.GetSection("JWTOptions").Get<JWTOptions>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = JWTOptions.Issuer,
                    ValidAudience = JWTOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTOptions.SecurityKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policyBuilder =>
                {
                    policyBuilder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            var app = builder.Build();


            using var ScopedServices = app.Services.CreateScope();
            var DbInitializer = ScopedServices.ServiceProvider.GetRequiredService<IDbInitializer>();
            await DbInitializer.InitializerAsync();
            await DbInitializer.InitializerIdentityAsync();

            app.UseMiddleware<GlobalErrorHandlingMiddleware>();

            app.UseStaticFiles();

            app.UseRouting();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();



            app.Run();
        }
    }
}
