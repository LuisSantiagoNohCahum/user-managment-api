using Microsoft.Extensions.FileProviders;

namespace ApiUsers.Extensions
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseBaseConfigurations(this IApplicationBuilder app, string rootFolder = "wwwroot", bool useStaticFiles = true, bool showSwaggerUi = true)
        {
            app.UseAuthorization();
            app.UseAuthentication();
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
            string fullRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            if (!Directory.Exists(fullRootPath)) Directory.CreateDirectory(fullRootPath);

            app.UseStaticFiles(new StaticFileOptions()
            {
                RequestPath = "/Private",
                ServeUnknownFileTypes = true,
                FileProvider = new PhysicalFileProvider(fullRootPath)
            });

            return app;
        }
    }
}
