using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelecomWeb.Controllers.tables;
using TelecomWeb.Models;
using Xunit;

namespace TelecomWeb.Tests.Controllers
{
    public class TariffPlansControllerTests : IDisposable
    {
        private readonly TelecomDbContext _context;
        private readonly TariffPlansController _controller;

        public TariffPlansControllerTests()
        {
            var options = new DbContextOptionsBuilder<TelecomDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TelecomDbContext(options);
            _controller = new TariffPlansController(_context);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithPaginatedList()
        {
            var tariffPlans = new List<TariffPlan>
            {
                new TariffPlan { TariffPlanId = 1, TariffName = "Basic", SubscriptionFee = 10.0m },
                new TariffPlan { TariffPlanId = 2, TariffName = "Premium", SubscriptionFee = 20.0m }
            };
            await _context.TariffPlans.AddRangeAsync(tariffPlans);
            await _context.SaveChangesAsync();

            var result = await _controller.Index(1, null, null, null, null, null) as ViewResult;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<PaginatedList<TariffPlan>>(result.Model);
            Assert.Equal(2, ((PaginatedList<TariffPlan>)result.Model).Count);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await _controller.Details(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithTariffPlan()
        {
            var tariffPlan = new TariffPlan { TariffPlanId = 1, TariffName = "Basic", SubscriptionFee = 10.0m };
            await _context.TariffPlans.AddAsync(tariffPlan);
            await _context.SaveChangesAsync();

            var result = await _controller.Details(1) as ViewResult;

            Assert.IsType<ViewResult>(result);
            Assert.Equal(tariffPlan, result.Model);
        }

        [Fact]
        public async Task Create_RedirectsToIndex_OnValidModel()
        {
            var tariffPlan = new TariffPlan { TariffPlanId = 1, TariffName = "Basic", SubscriptionFee = 10.0m };

            var result = await _controller.Create(tariffPlan) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
            Assert.Contains(tariffPlan, _context.TariffPlans);
        }

        [Fact]
        public async Task Create_ReturnsView_OnInvalidModel()
        {
            _controller.ModelState.AddModelError("TariffName", "Required");
            var tariffPlan = new TariffPlan();

            var result = await _controller.Create(tariffPlan) as ViewResult;

            Assert.IsType<ViewResult>(result);
            Assert.Equal(tariffPlan, result.Model);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenIdsDoNotMatch()
        {
            var tariffPlan = new TariffPlan { TariffPlanId = 1, TariffName = "Basic" };

            var result = await _controller.Edit(2, tariffPlan);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_RedirectsToIndex_OnValidModel()
        {
            var tariffPlan = new TariffPlan { TariffPlanId = 1, TariffName = "Basic", SubscriptionFee = 10.0m };
            await _context.TariffPlans.AddAsync(tariffPlan);
            await _context.SaveChangesAsync();

            tariffPlan.TariffName = "Updated Tariff";
            var result = await _controller.Edit(1, tariffPlan) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Updated Tariff", _context.TariffPlans.First().TariffName);
        }

        [Fact]
        public async Task DeleteConfirmed_RedirectsToIndex_OnSuccess()
        {
            var tariffPlan = new TariffPlan { TariffPlanId = 1, TariffName = "Basic", SubscriptionFee = 10.0m };
            await _context.TariffPlans.AddAsync(tariffPlan);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
            Assert.DoesNotContain(tariffPlan, _context.TariffPlans);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await _controller.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithTariffPlan()
        {
            var tariffPlan = new TariffPlan { TariffPlanId = 1, TariffName = "Basic" };
            await _context.TariffPlans.AddAsync(tariffPlan);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(1) as ViewResult;

            Assert.IsType<ViewResult>(result);
            Assert.Equal(tariffPlan, result.Model);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
