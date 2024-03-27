using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CustomerInfo.REST.DTOs;
using Microsoft.EntityFrameworkCore;
using CustomerInfo.REST.Data;
using Microsoft.Extensions.DependencyInjection;


namespace CustomerInfo.Test.Integration
{
    [Collection("Sequential")]
    public class CustomerInfo_Search_Tests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {

        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;

        private readonly string _userApiKey = "tWoNuTGFkErOBxl0RVZRZGs3LZwjCTtjDaJAQsmAwFBznufYj4QHfeOWALKFTlPO";
        private readonly string _accessToken;

        public CustomerInfo_Search_Tests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();

            var scope = factory.Services.GetService<IServiceScopeFactory>().CreateScope();
            _dbContext = scope.ServiceProvider.GetService<AppDbContext>();

            SeedDatabase().Wait();

            _accessToken = GetAccessToken(_userApiKey).Result; 
        }


        [Fact]
        public async Task GetCustomers_OK()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _httpClient.GetAsync("/api/CustomerInfo/all");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();

            Assert.NotNull(customers);  
            Assert.Equal(10, customers.Count);
        }


        [Fact]
        public async Task SearchCustomers_WithNoSearchText_ReturnsFiveCustomers()
        { 
            var response = await _httpClient.GetAsync("/api/CustomerInfo/search");
            var customerResult = await response.Content.ReadFromJsonAsync<CustomerSearchResult>();
            var customers = customerResult.Customers;

            Assert.Equal(5, customers.Count);
        }

        [Fact]
        public async Task SearchCustomers_WithSearchText_ReturnsMatchingCustomers()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/search?searchText=test1@");
            var customerResult = await response.Content.ReadFromJsonAsync<CustomerSearchResult>();
            var customers = customerResult.Customers;

            Assert.Single(customers);
            Assert.Equal("200001011001", customers.First().SSN);
        }

        [Fact]
        public async Task SearchCustomers_WithPaging_ReturnsCorrectCustomers()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/search?searchText=test&pageSize=3&page=2");
            var customerResult = await response.Content.ReadFromJsonAsync<CustomerSearchResult>();
            var customers = customerResult.Customers;

            Assert.Equal(3, customers.Count);
            Assert.Equal("200001011004", customers.First().SSN);
            Assert.Equal("200001011006", customers.Last().SSN);
        }

        [Fact]
        public async Task SearchCustomers_WithInvalidPaging_ReturnsEmptyList()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/search?searchText=test&pageSize=3&page=5");
            var customerResult = await response.Content.ReadFromJsonAsync<CustomerSearchResult>();
            var customers = customerResult.Customers;

            Assert.Empty(customers);
        }

        [Fact]
        public async Task SearchCustomers_WithInvalidSearchText_ReturnsEmptyList()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/search?searchText=invalid");
            var customerResult = await response.Content.ReadFromJsonAsync<CustomerSearchResult>();
            var customers = customerResult.Customers;

            Assert.Empty(customers);
        }


        [Fact]
        public async Task GetCustomerSearchSuggestions_ReturnsMatchingEmail()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/search/suggestions/test7");
            var suggestions = await response.Content.ReadFromJsonAsync<List<string>>();

            Assert.NotNull(suggestions);
            Assert.Single(suggestions);
            Assert.Equal("test7@gmail.com", suggestions.First());
        }

        [Fact]
        public async Task GetCustomerSearchSuggestions_ReturnsMatchingSSN()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/search/suggestions/11004");
            var suggestions = await response.Content.ReadFromJsonAsync<List<string>>();

            Assert.NotNull(suggestions);
            Assert.Single(suggestions);
            Assert.Equal("200001011004", suggestions.First());
        }

        [Fact]
        public async Task GetCustomerSearchSuggestions_ReturnsMatchingPhoneNumber()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/search/suggestions/10010");
            var suggestions = await response.Content.ReadFromJsonAsync<List<string>>();

            Assert.NotNull(suggestions);
            Assert.Single(suggestions);
            Assert.Equal("+46720010010", suggestions.First());
        }


        [Fact]
        public async Task GetCustomerSearchSuggestions_WithInvalidSearchText_ReturnsEmptyList()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/search/suggestions/invalid");
            var suggestions = await response.Content.ReadFromJsonAsync<List<string>>();

            Assert.NotNull(suggestions);
            Assert.Empty(suggestions);
        }

        [Fact]
        public async Task GetCustomerSearchSuggestions_WithNoSearchText_NotFound()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/search/suggestions/");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        void IDisposable.Dispose()
        {
            _httpClient.Dispose();
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        private async Task<string> GetAccessToken(string apiKey)
        {
            var apiKeyModel = new ApiKeyDto()
            {
                ApiKey = apiKey
            };
            var response = await _httpClient.PostAsJsonAsync<ApiKeyDto>("/api/auth/get-token/", apiKeyModel);
            return await response.Content.ReadAsStringAsync();
        }

        private async Task SeedDatabase()
        {
            _dbContext.Customers.Add(new Customer()
            {
                SSN = "200001011001",
                Name = "Test1 Testsson",
                Email = "test1@gmail.com",
                PhoneNumber = "+46720010001"
            });

            _dbContext.Customers.Add(new Customer()
            {
                SSN = "200001011002",
                Name = "Test2 Testsson",
                Email = "test2@gmail.com",
                PhoneNumber = "+46720010002"
            });

            _dbContext.Customers.Add(new Customer()
            {
                SSN = "200001011003",
                Name = "Test3 Testsson",
                Email = "test3@gmail.com",
                PhoneNumber = "+46720010003"
            });

            _dbContext.Customers.Add(new Customer()
            {
                SSN = "200001011004",
                Name = "Test4 Testsson",
                Email = "test4@gmail.com",
                PhoneNumber = "+46720010004"
            });

            _dbContext.Customers.Add(new Customer()
            {
                SSN = "200001011005",
                Name = "Test5 Testsson",
                Email = "test5@gmail.com",
                PhoneNumber = "+46720010005"
            });

            _dbContext.Customers.Add(new Customer()
            {
                SSN = "200001011006",
                Name = "Test6 Testsson",
                Email = "test6@gmail.com",
                PhoneNumber = "+46720010006"
            });

            _dbContext.Customers.Add(new Customer()
            {
                SSN = "200001011007",
                Name = "Test7 Testsson",
                Email = "test7@gmail.com",
                PhoneNumber = "+46720010007"
            });

            _dbContext.Customers.Add(new Customer()
            {
                SSN = "200001011008",
                Name = "Test8 Testsson",
                Email = "test8@gmail.com",
                PhoneNumber = "+46720010008"
            });

            _dbContext.Customers.Add(new Customer()
            {
                SSN = "200001011009",
                Name = "Test9 Testsson",
                Email = "test9@gmail.com",
                PhoneNumber = "+46720010009"
            });

            _dbContext.Customers.Add(new Customer()
            {
                SSN = "200001011010",
                Name = "Test10 Testsson",
                Email = "test10@gmail.com",
                PhoneNumber = "+46720010010"
            });

            await _dbContext.SaveChangesAsync();
        }


    }
}
