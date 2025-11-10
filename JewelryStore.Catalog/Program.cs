using JewelryStore.CatalogService.Catalog.DAL.Data;
using JewelryStore.CatalogService.Catalog.DAL.Repositories;
using JewelryStore.CatalogService.Catalog.DAL.Repositories.Interfaces;
using JewelryStore.CatalogService.Catalog.DAL.UOW;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IMetalRepository, MetalRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductStoneRepository, ProductStoneRepository>();
builder.Services.AddScoped<IStoneRepository, StoneRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Jewelry shop Catalog API",
        Version = "v1",
        Description = "API for jewelry shop catalog management"
    });
});

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

