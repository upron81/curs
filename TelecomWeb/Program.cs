using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TelecomWeb.Middleware;
using TelecomWeb.Models;
using TelecomWeb.Services;

namespace TelecomWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TelecomDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("TelecomDatabase")));
            builder.Services.AddMemoryCache();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddScoped<ICachedDataService, CachedDataService>();
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                .AddEntityFrameworkStores<TelecomDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();
            builder.Services.AddScoped<IdentitySeedData>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            app.UseSession();
            app.UseDbInitializer();
            app.UseResponseCaching();
            app.UseAuthentication();
            app.UseAuthorization();

            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IdentitySeedData>();
                initializer.Initialize();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>{endpoints.MapRazorPages();});

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
