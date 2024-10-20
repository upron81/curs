using Microsoft.EntityFrameworkCore;
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
            //builder.Services.AddScoped<ICachedDataService, CachedDataService>();
            // добавление поддержки сессии
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddScoped<ICachedDataService, CachedDataService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            app.UseSession();
            app.UseDbInitializer();
            app.UseResponseCaching(); 

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
