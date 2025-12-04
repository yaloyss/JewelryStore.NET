using FluentValidation.AspNetCore;
using MediatR;
using Reviews.API.Middleware;
using Reviews.Application.Behaviors;
using Reviews.Application.Commands.ReviewCommands.CreateReview;
using Reviews.Application.Services;
using Reviews.Domain.Interfaces;
using Reviews.Domain.Interfaces.Services;
using Reviews.Infrastructure.Mongo;
using Reviews.Infrastructure.Mongo.Cofiguration;
using Reviews.Infrastructure.Mongo.Mapping;
using Reviews.Infrastructure.Mongo.Repositories;
using Reviews.Infrastructure.Mongo.Seeder;
using Reviews.Infrastructure.Mongo.UOW;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IDiscussionRepository, DiscussionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IDiscussionService, DiscussionService>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(CreateReviewCommandHandler).Assembly);

    cfg.AddOpenBehavior(typeof(ExceptionHandlingBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddControllers().AddFluentValidation();

builder.Services.AddScoped<IDataSeeder, DatabaseSeeder>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.s
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Jewelry Shop Reviews API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await seeder.SeedAsync();
}

app.Run();

