using TelecomApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AspWeb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(Db8328Context db)
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
