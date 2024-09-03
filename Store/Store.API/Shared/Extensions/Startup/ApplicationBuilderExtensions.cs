using Microsoft.EntityFrameworkCore;
using Store.Infrastructure.Configuration;
using Store.Infrastructure.Persistence;
using Store.Infrastructure.Services.SignalR;

namespace Store.API.Shared.Extensions.Startup
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseMiddleware<JwtRefreshMiddleware>();


            return app;
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }

            return app;
        }

        public static async Task InitializeDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await dataSeeder.SeedDataAsync();

                var shopDbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
                await shopDbContext.Database.MigrateAsync();
            }
        }

        public static IApplicationBuilder UseCustomEndpoints(this IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseCors("MyAllowSpecificOrigins");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<PaymentHub>("/paymentHub");
            });


            return app;
        }
    }
}
