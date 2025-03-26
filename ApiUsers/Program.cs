using ApiUsers.Repository;
using FluentValidation;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Configure the API services here


// Add controllers
builder.Services.AddControllers();

// Configure the swagger docs generator
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Json Web Token for auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options=>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtToken:Issuer"],
        ValidAudience = builder.Configuration["JwtToken:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtToken:Key"] ?? ""))
    };
});

// Configure the EF database context
builder.Services.AddDbContext<AppDbContext>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase")));

// Configure to use Fluent Validatiors
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);

var app = builder.Build();

// Use the auth config
app.UseAuthorization();
app.UseAuthentication();

// Configure CORS Policy
app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

// Show swagger config
app.UseSwagger();
app.UseSwaggerUI();

// Enable use static files
string rootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
if(!Directory.Exists(rootFolder)) Directory.CreateDirectory(rootFolder);

app.UseStaticFiles(new StaticFileOptions() { 
    RequestPath = "/Private",
    ServeUnknownFileTypes = true,
    FileProvider = new PhysicalFileProvider(rootFolder)
});

// Map controllers
app.MapControllers();

app.Run();
