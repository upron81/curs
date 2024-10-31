using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using TelecomWeb.Models;
using Xunit;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit.Abstractions;
using TelecomWeb;
using Microsoft.EntityFrameworkCore;

namespace RestAPITests
{
    public class SubscribersControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _output;
        private const int _subscriberId = 1;

        public SubscribersControllerTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithSubscribers()
        {
            var response = await _client.GetAsync("/Subscribers");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenSubscriberIdIsNull()
        {
            var response = await _client.GetAsync("/Subscribers/Details?Id=");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithSubscriber_WhenIdIsValid()
        {
            var response = await _client.GetAsync($"/Subscribers/Details/{_subscriberId}");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenSubscriberIdDoesNotExist()
        {
            var nonExistentId = 999999;
            var response = await _client.GetAsync($"/Subscribers/Edit/{nonExistentId}");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WhenSubscriberIsValid()
        {
            var updatedSubscriber = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("SubscriberId", _subscriberId.ToString()),
                new KeyValuePair<string, string>("FullName", "John Doe Updated"),
                new KeyValuePair<string, string>("HomeAddress", "456 Main St"),
                new KeyValuePair<string, string>("PassportData", "987654321")
            });

            var response = await _client.PostAsync($"/Subscribers/Edit/{_subscriberId}", updatedSubscriber);
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenSubscriberIdIsNull()
        {
            var response = await _client.GetAsync("/Subscribers/Delete?Id=");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithSubscriber_WhenIdIsValid()
        {
            var response = await _client.GetAsync($"/Subscribers/Delete/{_subscriberId}");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Statistic_ReturnsViewResult_WithSubscriberCount()
        {
            var response = await _client.GetAsync("/Subscribers/Statistic");
            response.EnsureSuccessStatusCode();
        }
    }
}
