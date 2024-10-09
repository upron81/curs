using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TelecomApp.Models;
using System;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("TelecomDatabase");

        var optionsBuilder = new DbContextOptionsBuilder<Db8328Context>();
        optionsBuilder.UseSqlServer(connectionString);

        using (var context = new Db8328Context(optionsBuilder.Options))
        {
            var subscribers = context.Subscribers.ToList();
            Console.WriteLine("Все абоненты:");
            foreach (var subscriber in subscribers)
            {
                Console.WriteLine($"ID: {subscriber.SubscriberId}, ФИО: {subscriber.FullName}");
            }

            var filteredSubscribers = context.Subscribers
                .Where(s => s.HomeAddress.Contains("Москва"))
                .ToList();
            Console.WriteLine("\nАбоненты с адресом, содержащим 'Москва':");
            foreach (var subscriber in filteredSubscribers)
            {
                Console.WriteLine($"ID: {subscriber.SubscriberId}, ФИО: {subscriber.FullName}, Адрес: {subscriber.HomeAddress}");
            }

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

            var filteredContracts = context.Contracts
                .Where(c => c.SubscriberId == 1)
                .Include(c => c.Subscriber)
                .ToList();
            Console.WriteLine("\nКонтракты абонента с ID 1:");
            foreach (var contract in filteredContracts)
            {
                Console.WriteLine($"ID контракта: {contract.ContractId}, Телефон: {contract.PhoneNumber}");
            }

            var newSubscriber = new Subscriber
            {
                FullName = "Петров Петр Петрович",
                HomeAddress = "Улица Пушкина, дом 10",
                PassportData = "1234 567890"
            };
            context.Subscribers.Add(newSubscriber);
            context.SaveChanges();
            Console.WriteLine($"\nДобавлен новый абонент: ID = {newSubscriber.SubscriberId}, ФИО = {newSubscriber.FullName}");

            var existingStaff = context.Staff.FirstOrDefault();
            if (existingStaff == null)
            {
                Console.WriteLine("Нет доступных сотрудников для связывания с контрактом.");
                return;
            }

            var newContract = new Contract
            {
                SubscriberId = newSubscriber.SubscriberId,
                TariffPlanId = 1,
                ContractDate = DateTime.Now,
                PhoneNumber = "89001234567",
                StaffId = existingStaff.StaffId
            };
            context.Contracts.Add(newContract);
            context.SaveChanges();
            Console.WriteLine($"Добавлен новый контракт: ID = {newContract.ContractId}, Телефон = {newContract.PhoneNumber}");

            var subscriberToRemove = context.Subscribers.Find(newSubscriber.SubscriberId);
            if (subscriberToRemove != null)
            {
                context.Subscribers.Remove(subscriberToRemove);
                context.SaveChanges();
                Console.WriteLine($"Удален абонент: ID = {subscriberToRemove.SubscriberId}, ФИО = {subscriberToRemove.FullName}");
            }

            var contractToRemove = context.Contracts.Find(newContract.ContractId);
            if (contractToRemove != null)
            {
                context.Contracts.Remove(contractToRemove);
                context.SaveChanges();
                Console.WriteLine($"Удален контракт: ID = {contractToRemove.ContractId}, Телефон = {contractToRemove.PhoneNumber}");
            }

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
