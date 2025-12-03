using System;
using System.IO;
using Catalog.API.Middleware;
using Catalog.BLL.Mapper;
using Catalog.BLL.Services;
using Catalog.BLL.Services.Interfaces;
using Catalog.BLL.Validators;
using Catalog.DAL.Data;
using Catalog.DAL.Sorting;
using Catalog.DAL.UOW;
using Catalog.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build()).CreateLogger();

try
{
    Log.Information("Starting Catalog API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddDbContext<CatalogDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("CatalogDb")));

    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IMetalService, MetalService>();
    builder.Services.AddScoped<IStoneService, StoneService>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
    builder.Services.AddScoped<ISorting<Product>, Sorting<Product>>();

    builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDTOValidator>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Jewelry Shop Catalog API",
            Version = "v1",
            Description = "API for Jewelry Shop Catalog Management"
        });
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestPath", httpContext.Request.Path);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
        };
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionHandler>();
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Catalog API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}