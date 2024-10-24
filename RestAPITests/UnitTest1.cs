using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using TelecomWeb.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using RestAPI;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;

namespace RestAPITests
{
    public class ContractEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _output;

        public ContractEndpointsTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task GetAllContracts_ReturnsOkResult()
        {
            var response = await _client.GetAsync("/api/Contract");
            response.EnsureSuccessStatusCode();
            var contracts = await response.Content.ReadFromJsonAsync<List<Contract>>();
            Assert.NotNull(contracts);
        }

        [Fact]
        public async Task GetContractById_ReturnsOkResult()
        {
            var contractId = 1039;
            _output.WriteLine($"/api/Contract/{contractId}/");
            var response = await _client.GetAsync($"/api/Contract/{contractId}");
            response.EnsureSuccessStatusCode();
            var contract = await response.Content.ReadFromJsonAsync<Contract>();
            Assert.NotNull(contract);
            Assert.Equal(contractId, contract.ContractId);
        }

        [Fact]
        public async Task CreateContract_ReturnsCreatedResult()
        {
            var newContract = new Contract
            {
                SubscriberId = 4,
                TariffPlanId = 4,
                ContractDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ContractEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                PhoneNumber = "123456789",
                StaffId = 4
            };
            var response = await _client.PostAsJsonAsync("/api/Contract", newContract);
            response.EnsureSuccessStatusCode();
            var createdContract = await response.Content.ReadFromJsonAsync<Contract>();
            Assert.NotNull(createdContract);
            Assert.Equal(newContract.PhoneNumber, createdContract.PhoneNumber);
        }

        [Fact]
        public async Task UpdateContract_ReturnsOkResult()
        {
            var contractId = 1039;
            var updatedContract = new Contract
            {
                SubscriberId = 3,
                TariffPlanId = 3,
                ContractDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ContractEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                PhoneNumber = "987654321",
                StaffId = 3
            };
            var response = await _client.PutAsJsonAsync($"/api/Contract/{contractId}", updatedContract);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeleteContract_ReturnsOkResult()
        {
            var contractId = 1009;
            var response = await _client.DeleteAsync($"/api/Contract/{contractId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
