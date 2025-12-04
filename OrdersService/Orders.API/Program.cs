using Orders.API.Middleware;
using Orders.BLL.Mapper;
using Orders.BLL.Services;
using Orders.BLL.Services.Interfaces;
using Orders.DAL.UOW;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUnitOfWork>(sp =>
{
    var connectionstring = builder.Configuration.GetConnectionString("OrdersDB");
    return new UnitOfWork(connectionstring);
});


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

