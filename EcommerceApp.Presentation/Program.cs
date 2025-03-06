using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Services;
using EcommerceApp.Application.Utils;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Domain.Interfaces;
using EcommerceApp.Infrastructure.Data;
using EcommerceApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;
using System.Security.Claims;
using Product = EcommerceApp.Domain.Entities.Product;
using ProductService = EcommerceApp.Application.Services.ProductService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<EcommerceAppDbContext>(options
    => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(ApplicationMapper));
builder.Services.AddSingleton<IS3Service, S3Service>();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Value;

var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = domain;
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

builder.Services.AddScoped<IReposistory<Category>, Repository<Category>>();
builder.Services.AddScoped<IReposistory<Product>, Repository<Product>>();
builder.Services.AddScoped<IReposistory<ProductVariant>, Repository<ProductVariant>>();
builder.Services.AddScoped<IReposistory<Cart>, Repository<Cart>>();
builder.Services.AddScoped<IReposistory<CartItem>, Repository<CartItem>>();
builder.Services.AddScoped<IReposistory<Order>, Repository<Order>>();
builder.Services.AddScoped<IReposistory<OrderItem>, Repository<OrderItem>>();
builder.Services.AddScoped<IReposistory<FavouriteProduct>, Repository<FavouriteProduct>>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

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
