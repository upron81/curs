﻿using Microsoft.EntityFrameworkCore;
using System.Text;
using TelecomWeb.Models;

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
                string sql = File.ReadAllText(sqlFilePath, Encoding.UTF8);

                db.Database.ExecuteSqlRaw(sql);
            }
            else
            {
                Console.WriteLine($"Файл не найден: {sqlFilePath}");
            }
        }
    }
}
