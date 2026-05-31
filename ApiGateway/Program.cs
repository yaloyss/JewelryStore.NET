var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddFrontendCors(builder.Configuration);

var app = builder.Build();

app.UseFrontendCors();
app.MapReverseProxy();

app.Run();
