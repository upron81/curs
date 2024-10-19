using TelecomWeb.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelecomWeb.Models;

namespace TelecomWeb.Middleware
{
    public class DbInitializerMiddleware
    {
        private readonly RequestDelegate _next;

        public DbInitializerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, TelecomDbContext dbContext)
        {
            var hasSubscribers = dbContext.Subscribers.Any();

            if (!hasSubscribers)
            {
                DbInitializer.Initialize(dbContext);
            }

            await _next(context);
        }
    }

    public static class DbInitializerExtensions
    {
        public static IApplicationBuilder UseDbInitializer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DbInitializerMiddleware>();
        }
    }
}
