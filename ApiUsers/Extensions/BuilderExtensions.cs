using Microsoft.OpenApi.Models;

namespace ApiUsers.Extensions
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddBaseApiServices(this IServiceCollection services, ConfigurationManager configuration, Action<BuilderOptions>? options = null)
        {
            var builderOptions = new BuilderOptions();
            options?.Invoke(builderOptions);

            services.AddControllers();

            services.AddHttpContextAccessor();
            services.AddJwtConfiguration(configuration);
            services.AddSwaggerConfiguration(builderOptions);
            services.AddInfrastructureServices(configuration);
            services.AddApplicationServices();

            services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);

            return services;
        }

        public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtToken:Issuer"],
                    ValidAudience = configuration["JwtToken:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtToken:Key"] ?? ""))
                };
            });

            return services;
        }

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services, BuilderOptions options)
        {
            string? swaggerDocName = string.IsNullOrEmpty(options.SwaggerTittle) 
                ? typeof(Program).Assembly.GetName()?.Name 
                : options.SwaggerTittle;

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = options.SwaggerVersion ?? "v1.0",
                    Title = swaggerDocName,
                    Description = options.SwaggerDescription ?? $"An ASP.NET Core Web API for {swaggerDocName}"
                });
                c.ResolveConflictingActions(descriptions => descriptions.First());
            });

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<ILoginService, LoginService>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddScoped<IWebRootFilesHelper, WebRootFilesHelper>();
            services.AddScoped<IPasswordHasherHelper, PasswordHasherHelper>();
            services.AddScoped<IExcelHelper, ExcelHelper>();

            services.AddDbContext<AppDbContext>(e => e.UseSqlServer(configuration.GetConnectionString("MainConnectionString")));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRolRepository, RolRepository>();

            return services;
        }
    }
}
