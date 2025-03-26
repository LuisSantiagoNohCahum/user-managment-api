var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBaseApiServices(builder.Configuration);

var app = builder.Build();

app.UseBaseConfigurations();

app.MapControllers();

app.Run();
