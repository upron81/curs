using Microsoft.Extensions.Caching.Memory;
using TelecomWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TelecomWeb.Services
{
    public class CachedDataService : ICachedDataService
    {
        private readonly TelecomDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public CachedDataService(TelecomDbContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        public IEnumerable<Contract> GetContracts(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Contract> contracts))
            {
                contracts = _dbContext.Contracts.Take(rowsNumber).ToList();
                _memoryCache.Set(cacheKey, contracts, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(282)
                });
            }
            return contracts;
        }

        public IEnumerable<InternetUsage> GetInternetUsages(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<InternetUsage> internetUsages))
            {
                internetUsages = _dbContext.InternetUsages.Take(rowsNumber).ToList();
                _memoryCache.Set(cacheKey, internetUsages, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(282)
                });
            }
            return internetUsages;
        }

        public IEnumerable<Message> GetMessages(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Message> messages))
            {
                messages = _dbContext.Messages.Take(rowsNumber).ToList();
                _memoryCache.Set(cacheKey, messages, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(282)
                });
            }
            return messages;
        }

        public IEnumerable<Call> GetCalls(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Call> calls))
            {
                calls = _dbContext.Calls.Take(rowsNumber).ToList();
                _memoryCache.Set(cacheKey, calls, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(282)
                });
            }
            return calls;
        }

        public IEnumerable<Staff> GetStaff(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Staff> staff))
            {
                staff = _dbContext.Staff.Take(rowsNumber).ToList();
                _memoryCache.Set(cacheKey, staff, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(282)
                });
            }
            return staff;
        }

        public IEnumerable<StaffPosition> GetStaffPositions(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<StaffPosition> staffPositions))
            {
                staffPositions = _dbContext.StaffPositions.Take(rowsNumber).ToList();
                _memoryCache.Set(cacheKey, staffPositions, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(282)
                });
            }
            return staffPositions;
        }

        public IEnumerable<Subscriber> GetSubscribers(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Subscriber> subscribers))
            {
                subscribers = _dbContext.Subscribers.Take(rowsNumber).ToList();
                _memoryCache.Set(cacheKey, subscribers, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(282)
                });
            }
            return subscribers;
        }

        public IEnumerable<TariffPlan> GetTariffPlans(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<TariffPlan> tariffPlans))
            {
                tariffPlans = _dbContext.TariffPlans.Take(rowsNumber).ToList();
                _memoryCache.Set(cacheKey, tariffPlans, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(282)
                });
            }
            return tariffPlans;
        }
    }
}
