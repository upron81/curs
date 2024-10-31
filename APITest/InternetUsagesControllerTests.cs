using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using TelecomWeb.Controllers.tables;
using TelecomWeb.Models;

namespace TelecomWeb.Tests.Controllers
{
    public class InternetUsagesControllerTests : IDisposable
    {
        private readonly TelecomDbContext _context;
        private readonly InternetUsagesController _controller;

        public InternetUsagesControllerTests()
        {
            var options = new DbContextOptionsBuilder<TelecomDbContext>()
                .UseInMemoryDatabase(databaseName: "TestInternetUsageDatabase")
                .Options;

            _context = new TelecomDbContext(options);
            _controller = new InternetUsagesController(_context);

            InitializeData();
        }

        private void InitializeData()
        {
            var subscriber = new Subscriber
            {
                FullName = "Иванов Иван Иванович",
                HomeAddress = "Улица Пушкина, дом 1",
                PassportData = "1234 567890"
            };

            var tariffPlan = new TariffPlan
            {
                TariffName = "Тариф 1",
                SubscriptionFee = 100,
                LocalCallRate = 1,
                LongDistanceCallRate = 2,
                InternationalCallRate = 3,
                IsPerSecond = true,
                SmsRate = 0.5m,
                MmsRate = 1,
                DataRatePerMb = 0.01m
            };

            _context.Subscribers.Add(subscriber);
            _context.TariffPlans.Add(tariffPlan);
            _context.SaveChanges();

            var contract = new Contract
            {
                SubscriberId = subscriber.SubscriberId,
                TariffPlanId = tariffPlan.TariffPlanId,
                ContractDate = DateOnly.FromDateTime(DateTime.Now),
                PhoneNumber = "9000000000"
            };

            _context.Contracts.Add(contract);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithInternetUsages()
        {
            var internetUsage = new InternetUsage
            {
                ContractId = _context.Contracts.First().ContractId,
                UsageDate = DateTime.Now,
                DataSentMb = 500,
                DataReceivedMb = 300
            };

            _context.InternetUsages.Add(internetUsage);
            await _context.SaveChangesAsync();

            var result = await _controller.Index(null, null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<InternetUsage>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithInternetUsage()
        {
            var internetUsage = new InternetUsage
            {
                ContractId = _context.Contracts.First().ContractId,
                UsageDate = DateTime.Now,
                DataSentMb = 500,
                DataReceivedMb = 300
            };

            _context.InternetUsages.Add(internetUsage);
            await _context.SaveChangesAsync();

            var result = await _controller.Details(internetUsage.UsageId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<InternetUsage>(viewResult.Model);
            Assert.Equal(internetUsage.UsageId, model.UsageId);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenInternetUsageDoesNotExist()
        {
            var result = await _controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsRedirectToActionResult_WhenModelIsValid()
        {
            var internetUsage = new InternetUsage
            {
                ContractId = _context.Contracts.First().ContractId,
                UsageDate = DateTime.Now,
                DataSentMb = 500,
                DataReceivedMb = 300
            };

            var result = await _controller.Create(internetUsage);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WhenModelIsInvalid()
        {
            _controller.ModelState.AddModelError("DataSentMb", "Required");

            var internetUsage = new InternetUsage
            {
                ContractId = _context.Contracts.First().ContractId,
                UsageDate = DateTime.Now
            };

            var result = await _controller.Create(internetUsage);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(internetUsage, viewResult.Model);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenInternetUsageDoesNotExist()
        {
            var result = await _controller.Edit(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WithInternetUsage()
        {
            var internetUsage = new InternetUsage
            {
                ContractId = _context.Contracts.First().ContractId,
                UsageDate = DateTime.Now,
                DataSentMb = 500,
                DataReceivedMb = 300
            };

            _context.InternetUsages.Add(internetUsage);
            await _context.SaveChangesAsync();

            var result = await _controller.Edit(internetUsage.UsageId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<InternetUsage>(viewResult.Model);
            Assert.Equal(internetUsage.UsageId, model.UsageId);
        }

        [Fact]
        public async Task Edit_ReturnsRedirectToActionResult_WhenModelIsValid()
        {
            var internetUsage = new InternetUsage
            {
                ContractId = _context.Contracts.First().ContractId,
                UsageDate = DateTime.Now,
                DataSentMb = 500,
                DataReceivedMb = 300
            };

            _context.InternetUsages.Add(internetUsage);
            await _context.SaveChangesAsync();

            internetUsage.DataSentMb = 600;

            var result = await _controller.Edit(internetUsage.UsageId, internetUsage);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithInternetUsage()
        {
            var internetUsage = new InternetUsage
            {
                ContractId = _context.Contracts.First().ContractId,
                UsageDate = DateTime.Now,
                DataSentMb = 500,
                DataReceivedMb = 300
            };

            _context.InternetUsages.Add(internetUsage);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(internetUsage.UsageId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<InternetUsage>(viewResult.Model);
            Assert.Equal(internetUsage.UsageId, model.UsageId);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_WhenInternetUsageExists()
        {
            var internetUsage = new InternetUsage
            {
                ContractId = _context.Contracts.First().ContractId,
                UsageDate = DateTime.Now,
                DataSentMb = 500,
                DataReceivedMb = 300
            };

            _context.InternetUsages.Add(internetUsage);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteConfirmed(internetUsage.UsageId);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Null(await _context.InternetUsages.FindAsync(internetUsage.UsageId));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
