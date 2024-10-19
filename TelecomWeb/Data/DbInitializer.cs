using TelecomWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace TelecomWeb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TelecomDbContext db)
        {
            db.Database.EnsureCreated();

            string sqlFilePath = "sql/insert.sql";

            if (File.Exists(sqlFilePath))
            {
                string sql = File.ReadAllText(sqlFilePath);

                db.Database.ExecuteSqlRaw(sql);
            }
            else
            {
                Console.WriteLine($"Файл не найден: {sqlFilePath}");
            }
        }
    }
}
