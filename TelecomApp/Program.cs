using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TelecomApp.Models;
using System;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Настройка конфигурации для чтения строки подключения из файла appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("TelecomDatabase");

        // Настройка DbContextOptions для использования строки подключения
        var optionsBuilder = new DbContextOptionsBuilder<Db8328Context>();
        optionsBuilder.UseSqlServer(connectionString);

        using (var context = new Db8328Context(optionsBuilder.Options))
        {
            // 1. Выборка всех данных из таблицы на стороне отношения «один» (Subscribers)
            var subscribers = context.Subscribers.ToList();
            Console.WriteLine("Все абоненты:");
            foreach (var subscriber in subscribers)
            {
                Console.WriteLine($"ID: {subscriber.SubscriberId}, ФИО: {subscriber.FullName}");
            }

            // 2. Выборка данных из таблицы Subscribers, отфильтрованные по определенному условию (например, по домашнему адресу)
            var filteredSubscribers = context.Subscribers
                .Where(s => s.HomeAddress.Contains("Москва")) // Например, выбираем абонентов из Москвы
                .ToList();
            Console.WriteLine("\nАбоненты с адресом, содержащим 'Москва':");
            foreach (var subscriber in filteredSubscribers)
            {
                Console.WriteLine($"ID: {subscriber.SubscriberId}, ФИО: {subscriber.FullName}, Адрес: {subscriber.HomeAddress}");
            }

            // 3. Выборка данных, сгруппированных по тарифным планам и подсчет количества абонентов в каждом тарифном плане
            var groupedSubscribers = context.Contracts
                .GroupBy(c => c.TariffPlanId)
                .Select(g => new
                {
                    TariffPlanId = g.Key,
                    Count = g.Count()
                }).ToList();
            Console.WriteLine("\nКоличество контрактов по тарифным планам:");
            foreach (var group in groupedSubscribers)
            {
                Console.WriteLine($"Тарифный план ID: {group.TariffPlanId}, Количество контрактов: {group.Count}");
            }

            // 4. Выборка данных из двух связанных таблиц (Subscribers и Contracts)
            var subscribersWithContracts = context.Subscribers
                .Include(s => s.Contracts)
                .Select(s => new
                {
                    SubscriberName = s.FullName,
                    Contracts = s.Contracts.Select(c => c.PhoneNumber)
                }).ToList();

            Console.WriteLine("\nАбоненты и их контракты:");
            foreach (var subscriber in subscribersWithContracts)
            {
                Console.WriteLine($"Абонент: {subscriber.SubscriberName}");
                foreach (var contract in subscriber.Contracts)
                {
                    Console.WriteLine($"  Телефон: {contract}");
                }
            }

            // 5. Выборка данных из двух таблиц с фильтрацией (Contracts по SubscriberId)
            var filteredContracts = context.Contracts
                .Where(c => c.SubscriberId == 1) // Например, выбираем контракты абонента с ID = 1
                .Include(c => c.Subscriber)
                .ToList();
            Console.WriteLine("\nКонтракты абонента с ID 1:");
            foreach (var contract in filteredContracts)
            {
                Console.WriteLine($"ID контракта: {contract.ContractId}, Телефон: {contract.PhoneNumber}");
            }

            // 6. Вставка данных в таблицу Subscribers (сторона отношения «Один»)
            var newSubscriber = new Subscriber
            {
                FullName = "Петров Петр Петрович",
                HomeAddress = "Улица Пушкина, дом 10",
                PassportData = "1234 567890"
            };
            context.Subscribers.Add(newSubscriber);
            context.SaveChanges();
            Console.WriteLine($"\nДобавлен новый абонент: ID = {newSubscriber.SubscriberId}, ФИО = {newSubscriber.FullName}");

            // Получаем существующий StaffID
            var existingStaff = context.Staff.FirstOrDefault(); // Получаем первого сотрудника
            if (existingStaff == null)
            {
                Console.WriteLine("Нет доступных сотрудников для связывания с контрактом.");
                return; // Прерываем выполнение, если нет сотрудников
            }

            // 7. Вставка данных в таблицу Contracts (сторона отношения «Многие»)
            var newContract = new Contract
            {
                SubscriberId = newSubscriber.SubscriberId,
                TariffPlanId = 1, // Предполагается, что тарифный план с ID = 1 существует
                ContractDate = DateTime.Now,
                PhoneNumber = "89001234567",
                StaffId = existingStaff.StaffId // Используем существующий StaffID
            };
            context.Contracts.Add(newContract);
            context.SaveChanges();
            Console.WriteLine($"Добавлен новый контракт: ID = {newContract.ContractId}, Телефон = {newContract.PhoneNumber}");

            // 8. Удаление данных из таблицы Subscribers
            var subscriberToRemove = context.Subscribers.Find(newSubscriber.SubscriberId);
            if (subscriberToRemove != null)
            {
                context.Subscribers.Remove(subscriberToRemove);
                context.SaveChanges();
                Console.WriteLine($"Удален абонент: ID = {subscriberToRemove.SubscriberId}, ФИО = {subscriberToRemove.FullName}");
            }

            // 9. Удаление данных из таблицы Contracts
            var contractToRemove = context.Contracts.Find(newContract.ContractId);
            if (contractToRemove != null)
            {
                context.Contracts.Remove(contractToRemove);
                context.SaveChanges();
                Console.WriteLine($"Удален контракт: ID = {contractToRemove.ContractId}, Телефон = {contractToRemove.PhoneNumber}");
            }

            // 10. Обновление записей в таблице Subscribers, где ID = 1
            var subscriberToUpdate = context.Subscribers
                .FirstOrDefault(s => s.SubscriberId == 1);
            if (subscriberToUpdate != null)
            {
                subscriberToUpdate.FullName = "Обновленное Имя";
                context.SaveChanges();
                Console.WriteLine($"Обновлено имя абонента: ID = {subscriberToUpdate.SubscriberId}, Новое ФИО = {subscriberToUpdate.FullName}");
            }
        }
    }
}
