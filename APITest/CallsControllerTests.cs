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
    public class CallsControllerTests : IDisposable
    {
        private readonly TelecomDbContext _context;
        private readonly CallsController _controller;

        public CallsControllerTests()
        {
            var options = new DbContextOptionsBuilder<TelecomDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new TelecomDbContext(options);
            _controller = new CallsController(_context);

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
        public async Task Index_ReturnsViewResult_WithCalls()
        {
            var call = new Call
            {
                ContractId = _context.Contracts.First().ContractId,
                CallDate = DateTime.Now,
                CallDuration = 120
            };

            _context.Calls.Add(call);
            await _context.SaveChangesAsync();

            var result = await _controller.Index(null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<Call>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithCall()
        {
            var call = new Call
            {
                ContractId = _context.Contracts.First().ContractId,
                CallDate = DateTime.Now,
                CallDuration = 120
            };

            _context.Calls.Add(call);
            await _context.SaveChangesAsync();

            var result = await _controller.Details(call.CallId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Call>(viewResult.Model);
            Assert.Equal(call.CallId, model.CallId);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenCallDoesNotExist()
        {
            var result = await _controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsRedirectToActionResult_WhenModelIsValid()
        {
            var call = new Call
            {
                ContractId = _context.Contracts.First().ContractId,
                CallDate = DateTime.Now,
                CallDuration = 120
            };

            var result = await _controller.Create(call);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WhenModelIsInvalid()
        {
            _controller.ModelState.AddModelError("CallDuration", "Required");

            var call = new Call
            {
                ContractId = _context.Contracts.First().ContractId,
                CallDate = DateTime.Now
            };

            var result = await _controller.Create(call);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(call, viewResult.Model);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenCallDoesNotExist()
        {
            var result = await _controller.Edit(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WithCall()
        {
            var call = new Call
            {
                ContractId = _context.Contracts.First().ContractId,
                CallDate = DateTime.Now,
                CallDuration = 120
            };

            _context.Calls.Add(call);
            await _context.SaveChangesAsync();

            var result = await _controller.Edit(call.CallId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Call>(viewResult.Model);
            Assert.Equal(call.CallId, model.CallId);
        }

        [Fact]
        public async Task Edit_ReturnsRedirectToActionResult_WhenModelIsValid()
        {
            var call = new Call
            {
                ContractId = _context.Contracts.First().ContractId,
                CallDate = DateTime.Now,
                CallDuration = 120
            };

            _context.Calls.Add(call);
            await _context.SaveChangesAsync();

            call.CallDuration = 130;

            var result = await _controller.Edit(call.CallId, call);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithCall()
        {
            var call = new Call
            {
                ContractId = _context.Contracts.First().ContractId,
                CallDate = DateTime.Now,
                CallDuration = 120
            };

            _context.Calls.Add(call);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(call.CallId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Call>(viewResult.Model);
            Assert.Equal(call.CallId, model.CallId);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_WhenCallExists()
        {
            var call = new Call
            {
                ContractId = _context.Contracts.First().ContractId,
                CallDate = DateTime.Now,
                CallDuration = 120
            };

            _context.Calls.Add(call);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteConfirmed(call.CallId);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Null(await _context.Calls.FindAsync(call.CallId));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
