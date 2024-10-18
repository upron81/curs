using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TelecomApp.Models;
using TelecomApp.Services;
using AspWeb.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using AspWeb.Middleware;

namespace AspWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<Db8328Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("TelecomDatabase")));

            builder.Services.AddMemoryCache();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ICachedDataService, CachedDataService>();

            // добавление поддержки сессии
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();
            app.UseDbInitializer();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
