using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace ApiUsers.Extensions
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseBaseConfigurations(this IApplicationBuilder app, string rootFolder = "wwwroot", bool useStaticFiles = true, bool showSwaggerUi = true)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCustomCorsPolicy();
            app.UseHttpsRedirection();

            if (useStaticFiles)
                app.UseCustomStaticFiles(rootFolder);

            if (showSwaggerUi)
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            return app;
        }

        public static IApplicationBuilder UseCustomCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            return app;
        }

        public static IApplicationBuilder UseCustomStaticFiles(this IApplicationBuilder app, string rootFolder)
        {
            string fullRootPath = Path.Combine(Directory.GetCurrentDirectory(), rootFolder);

            if (!Directory.Exists(fullRootPath)) Directory.CreateDirectory(fullRootPath);

            app.UseStaticFiles(new StaticFileOptions()
            {
                RequestPath = "/Public",
                ServeUnknownFileTypes = true,
                FileProvider = new PhysicalFileProvider(fullRootPath)
            });

            return app;
        }

        public static IApplicationBuilder UseErrorMidleware(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        var environment = context.RequestServices.CreateScope()
                            .ServiceProvider
                            .GetRequiredService<IHostEnvironment>();

                        string message = environment.IsDevelopment() ? contextFeature.Error.Message : "Internal server error";

                        var response = contextFeature.Error is ApiException ex
                            ? ApiResponse<string>.FailResponse(ex.DisplayMessage)
                            : ApiResponse<string>.FailResponse(message, environment.IsDevelopment() ? [contextFeature.Error.StackTrace] : []);
                            

                        var options = new JsonSerializerSettings() 
                        { 
                            ContractResolver = new CamelCasePropertyNamesContractResolver() 
                        };

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(response, options));
                    }
                });
            });

            return app;
        }
    }
}
