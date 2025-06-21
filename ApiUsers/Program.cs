var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBaseApiServices(builder.Configuration);

var app = builder.Build();

app.UseBaseConfigurations();

// dotnet ef migrations add Initial -> to generate migration file
// dotnet ef database update -> to apply changes in database structs

using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await dbContext.Database.MigrateAsync();

app.MapControllers();

app.Run();
