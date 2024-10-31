using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelecomWeb.Controllers.tables;
using TelecomWeb.Models;
using Xunit;

namespace TelecomWeb.Tests.Controllers
{
    public class MessagesControllerTests : IDisposable
    {
        private readonly TelecomDbContext _context;
        private readonly MessagesController _controller;

        public MessagesControllerTests()
        {
            var options = new DbContextOptionsBuilder<TelecomDbContext>()
                .UseInMemoryDatabase(databaseName: "TestMessagesDatabase")
                .Options;

            _context = new TelecomDbContext(options);
            _controller = new MessagesController(_context);

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
        public async Task Index_ReturnsViewResult_WithMessages()
        {
            var message = new Message
            {
                ContractId = _context.Contracts.First().ContractId,
                MessageDate = DateTime.Now,
                IsMms = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var result = await _controller.Index(null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<Message>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithMessage()
        {
            var message = new Message
            {
                ContractId = _context.Contracts.First().ContractId,
                MessageDate = DateTime.Now,
                IsMms = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var result = await _controller.Details(message.MessageId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Message>(viewResult.Model);
            Assert.Equal(message.MessageId, model.MessageId);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenMessageDoesNotExist()
        {
            var result = await _controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsRedirectToActionResult_WhenModelIsValid()
        {
            var message = new Message
            {
                ContractId = _context.Contracts.First().ContractId,
                MessageDate = DateTime.Now,
                IsMms = false
            };

            var result = await _controller.Create(message);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WhenModelIsInvalid()
        {
            _controller.ModelState.AddModelError("MessageDate", "Required");

            var message = new Message
            {
                ContractId = _context.Contracts.First().ContractId
            };

            var result = await _controller.Create(message);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(message, viewResult.Model);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenMessageDoesNotExist()
        {
            var result = await _controller.Edit(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WithMessage()
        {
            var message = new Message
            {
                ContractId = _context.Contracts.First().ContractId,
                MessageDate = DateTime.Now,
                IsMms = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var result = await _controller.Edit(message.MessageId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Message>(viewResult.Model);
            Assert.Equal(message.MessageId, model.MessageId);
        }

        [Fact]
        public async Task Edit_ReturnsRedirectToActionResult_WhenModelIsValid()
        {
            var message = new Message
            {
                ContractId = _context.Contracts.First().ContractId,
                MessageDate = DateTime.Now,
                IsMms = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            message.IsMms = true;

            var result = await _controller.Edit(message.MessageId, message);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithMessage()
        {
            var message = new Message
            {
                ContractId = _context.Contracts.First().ContractId,
                MessageDate = DateTime.Now,
                IsMms = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(message.MessageId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Message>(viewResult.Model);
            Assert.Equal(message.MessageId, model.MessageId);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_WhenMessageExists()
        {
            var message = new Message
            {
                ContractId = _context.Contracts.First().ContractId,
                MessageDate = DateTime.Now,
                IsMms = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteConfirmed(message.MessageId);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Null(await _context.Messages.FindAsync(message.MessageId));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
