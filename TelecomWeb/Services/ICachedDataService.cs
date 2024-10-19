using TelecomWeb.Models;
using System.Collections.Generic;

namespace TelecomWeb.Services
{
    public interface ICachedDataService
    {
        IEnumerable<Contract> GetContracts(string cacheKey, int rowsNumber = 20);
        IEnumerable<InternetUsage> GetInternetUsages(string cacheKey, int rowsNumber = 20);
        IEnumerable<Message> GetMessages(string cacheKey, int rowsNumber = 20);
        IEnumerable<Call> GetCalls(string cacheKey, int rowsNumber = 20);
        IEnumerable<Staff> GetStaff(string cacheKey, int rowsNumber = 20);
        IEnumerable<StaffPosition> GetStaffPositions(string cacheKey, int rowsNumber = 20);
        IEnumerable<Subscriber> GetSubscribers(string cacheKey, int rowsNumber = 20);
        IEnumerable<TariffPlan> GetTariffPlans(string cacheKey, int rowsNumber = 20);
    }
}
