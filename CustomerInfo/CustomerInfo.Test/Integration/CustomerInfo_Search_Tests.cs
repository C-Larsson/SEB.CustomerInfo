using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CustomerInfo.Test.Integration 
{
    public class CustomerInfo_Search_Tests : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        private readonly string _userApiKey = "tWoNuTGFkErOBxl0RVZRZGs3LZwjCTtjDaJAQsmAwFBznufYj4QHfeOWALKFTlPO";
        private readonly string _accessToken;

        public CustomerInfo_Search_Tests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
            _accessToken = GetAccessToken(_userApiKey).Result;

            SeedDatabase().Wait();
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

     

        private async Task<string> GetAccessToken(string apiKey)
        {
            var apiKeyModel = new ApiKeyModel()
            {
                ApiKey = apiKey
            };
            var response = await _httpClient.PostAsJsonAsync<ApiKeyModel>("/api/auth/get-token/", apiKeyModel);
            return await response.Content.ReadAsStringAsync();
        }

        private async Task SeedDatabase()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            await _httpClient.PostAsJsonAsync("/api/CustomerInfo", new Customer()
            {
                SSN = "200001011002",
                Email = "test2@gmail.com",
                PhoneNumber = "0720010002"
            });

            await _httpClient.PostAsJsonAsync("/api/CustomerInfo", new Customer()
            {
                SSN = "200001011003",
                Email = "test3@gmail.com",
                PhoneNumber = "0720010003"
            });

            await _httpClient.PostAsJsonAsync("/api/CustomerInfo", new Customer()
            {
                SSN = "200001011004",
                Email = "test4@gmail.com",
                PhoneNumber = "0720010004"
            });

            await _httpClient.PostAsJsonAsync("/api/CustomerInfo", new Customer()
            {
                SSN = "200001011005",
                Email = "test5@gmail.com",
                PhoneNumber = "0720010005"
            });

            await _httpClient.PostAsJsonAsync("/api/CustomerInfo", new Customer()
            {
                SSN = "200001011006",
                Email = "test6@gmail.com",
                PhoneNumber = "0720010006"
            });

            await _httpClient.PostAsJsonAsync("/api/CustomerInfo", new Customer()
            {
                SSN = "200001011007",
                Email = "test7@gmail.com",
                PhoneNumber = "0720010007"
            });

            await _httpClient.PostAsJsonAsync("/api/CustomerInfo", new Customer()
            {
                SSN = "200001011008",
                Email = "test8@gmail.com",
                PhoneNumber = "0720010008"
            });

            await _httpClient.PostAsJsonAsync("/api/CustomerInfo", new Customer()
            {
                SSN = "200001011009",
                Email = "test9@gmail.com",
                PhoneNumber = "0720010009"
            });

            await _httpClient.PostAsJsonAsync("/api/CustomerInfo", new Customer()
            {
                SSN = "200001011010",
                Email = "test10@gmail.com",
                PhoneNumber = "0720010010"
            });

        }


    }
}
