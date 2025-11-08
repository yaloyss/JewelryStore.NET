using JewelryStore.OrdersService.Orders.API.Middleware;
using JewelryStore.OrdersService.Orders.Application.Mapper;
using JewelryStore.OrdersService.Orders.Application.Services;
using JewelryStore.OrdersService.Orders.Application.Services.Interfaces;
using JewelryStore.OrdersService.Orders.BLL.Services;
using JewelryStore.OrdersService.Orders.BLL.Services.Interfaces;
using JewelryStore.OrdersService.Orders.DAL.Repositories;
using JewelryStore.OrdersService.Orders.DAL.Repositories.Interfaces;
using JewelryStore.OrdersService.Orders.DAL.UOW;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Jewelry shop Order API",
        Version = "v1",
        Description = "API for jewelry shop order management"
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
app.UseMiddleware<ExceptionHandler>();
app.UseAuthorization();

app.MapControllers();

app.Run();

