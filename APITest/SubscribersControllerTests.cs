using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelecomWeb.Controllers.tables;
using TelecomWeb.Models;
using Xunit;

namespace TelecomWeb.Tests.Controllers
{
    public class SubscribersControllerTests : IDisposable
    {
        private readonly TelecomDbContext _context;
        private readonly SubscribersController _controller;

        public SubscribersControllerTests()
        {
            var options = new DbContextOptionsBuilder<TelecomDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TelecomDbContext(options);
            _controller = new SubscribersController(_context);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await _controller.Details(null, null, null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithSubscriber()
        {
            var subscriber = new Subscriber { SubscriberId = 1, FullName = "John Doe", HomeAddress = "123 Street" };
            await _context.Subscribers.AddAsync(subscriber);
            await _context.SaveChangesAsync();

            var result = await _controller.Details(1, null, null) as ViewResult;

            Assert.IsType<ViewResult>(result);
            Assert.Equal(subscriber, result.Model);
        }

        [Fact]
        public async Task Create_RedirectsToIndex_OnValidModel()
        {
            var subscriber = new Subscriber { SubscriberId = 1, FullName = "John Doe" };

            var result = await _controller.Create(subscriber) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
            Assert.Contains(subscriber, _context.Subscribers);
        }

        [Fact]
        public async Task Create_ReturnsView_OnInvalidModel()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var subscriber = new Subscriber();

            var result = await _controller.Create(subscriber) as ViewResult;

            Assert.IsType<ViewResult>(result);
            Assert.Equal(subscriber, result.Model);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenIdsDoNotMatch()
        {
            var subscriber = new Subscriber { SubscriberId = 1, FullName = "John Doe" };

            var result = await _controller.Edit(2, subscriber);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_RedirectsToIndex_OnValidModel()
        {
            var subscriber = new Subscriber { SubscriberId = 1, FullName = "John Doe" };
            await _context.Subscribers.AddAsync(subscriber);
            await _context.SaveChangesAsync();

            subscriber.FullName = "Updated Name";
            var result = await _controller.Edit(1, subscriber) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Updated Name", _context.Subscribers.First().FullName);
        }

        [Fact]
        public async Task DeleteConfirmed_RedirectsToIndex_OnSuccess()
        {
            var subscriber = new Subscriber { SubscriberId = 1, FullName = "John Doe" };
            await _context.Subscribers.AddAsync(subscriber);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
            Assert.DoesNotContain(subscriber, _context.Subscribers);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await _controller.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithSubscriber()
        {
            var subscriber = new Subscriber { SubscriberId = 1, FullName = "John Doe" };
            await _context.Subscribers.AddAsync(subscriber);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(1) as ViewResult;

            Assert.IsType<ViewResult>(result);
            Assert.Equal(subscriber, result.Model);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
