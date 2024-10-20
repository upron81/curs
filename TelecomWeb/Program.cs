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
            Console.WriteLine("before");
            Console.WriteLine(builder.Configuration.GetConnectionString("TelecomDatabase"));
            Console.WriteLine("after");
            builder.Services.AddMemoryCache();
            //builder.Services.AddScoped<ICachedDataService, CachedDataService>();
            // ���������� ��������� ������
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            app.UseSession();
            app.UseDbInitializer();

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
