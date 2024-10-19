using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace TelecomWeb
{
    public static class SessionExtensions
    {
        // Запись произвольного объекта в сессию
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Считывание произвольного объекта из сессии
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
