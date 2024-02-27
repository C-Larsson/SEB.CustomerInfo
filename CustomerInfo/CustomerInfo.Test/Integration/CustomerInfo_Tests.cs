using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CustomerInfo.Test.Integration
{
    public class CustomerInfo_Tests : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        public CustomerInfo_Tests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
            // Seed database with test data
            _httpClient.PostAsJsonAsync("/api/CustomerInfo", new Customer()
            {
                SSN = "197210161234",
                Email = "test@gmail.com",
                PhoneNumber = "+46701234567"
            }).Wait();
        }

        [Fact]
        public async Task GetCustomer_OK()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/197210161234");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var customer = await response.Content.ReadFromJsonAsync<Customer>();
            Assert.NotNull(customer);
            Assert.Equal("197210161234", customer.SSN);
            Assert.Equal("test@gmail.com", customer.Email);
            Assert.Equal("+46701234567", customer.PhoneNumber);
        }

        [Fact]
        public async Task GetCustomer_NotFound()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/197210161230"); // SSN does not exist
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateCustomer_Ok()
        {
            var customer = new Customer()
            {
                SSN = "197210161235",
                PhoneNumber = "0701234567"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var customerInfoResponse = await response.Content.ReadFromJsonAsync<Customer>();
            Assert.NotNull(customerInfoResponse);
            Assert.Equal("197210161235", customerInfoResponse.SSN);
            Assert.Equal("+46701234567", customerInfoResponse.PhoneNumber);
        }

        [Fact]
        public async Task CreateCustomer_Conflict()
        {
            var customer = new Customer()
            {
                SSN = "197210161234", // SSN already exists
                PhoneNumber = "0701234567"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task CreateCustomer_BadRequest_SSN()
        {
            var customer = new Customer()
            {
                SSN = "19721016123", // Invalid SSN
                PhoneNumber = "0701234567"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCustomer_Ok()
        {
            var customer = new Customer()
            {
                SSN = "197210161234",
                PhoneNumber = "0701234567"
            };

            var response = await _httpClient.PutAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var customerInfoResponse = await response.Content.ReadFromJsonAsync<Customer>();
            Assert.NotNull(customerInfoResponse);
            Assert.Equal("197210161234", customerInfoResponse.SSN);
            Assert.Equal("+46701234567", customerInfoResponse.PhoneNumber);
        }

        [Fact]
        public async Task UpdateCustomer_BadRequest_SSN()
        {
            var customer = new Customer()
            {
                SSN = "19721016123", // Invalid SSN
                PhoneNumber = "07012345678"
            };

            var response = await _httpClient.PutAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCustomer_NotFound()
        {
            var customer = new Customer()
            {
                SSN = "197210161230", // SSN does not exist
                PhoneNumber = "0701234567"
            };

            var response = await _httpClient.PutAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCustomer_NoContent()
        {
            var response = await _httpClient.DeleteAsync("/api/CustomerInfo/197210161234");
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCustomer_NotFound()
        {
            var response = await _httpClient.DeleteAsync("/api/CustomerInfo/197210161230"); // SSN does not exist
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCustomers_Admin_OK()
        {
            // Prepare
            var apiKeyModel = new ApiKeyModel()
            {
                // This is the key for the admin role in appsettings.json
                ApiKey = "9m6KY9Q9aLFGiVsYeaSkQXu6hlgMnWrojzeOSuRiXvQOAHkvEfS9AmzWRadLOP9m"
            };
            var response1 = await _httpClient.PostAsJsonAsync<ApiKeyModel>("/api/auth/get-token/", apiKeyModel);
            var accessToken = await response1.Content.ReadAsStringAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("/api/CustomerInfo/admin");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();
            Assert.NotNull(customers);
            Assert.Equal(1, customers.Count);
        }

        [Fact]
        public async Task GetCustomers_User_Forbidden()
        {
            // Prepare
            var apiKeyModel = new ApiKeyModel()
            {
                // This is the key for the user role in appsettings.json
                ApiKey = "tWoNuTGFkErOBxl0RVZRZGs3LZwjCTtjDaJAQsmAwFBznufYj4QHfeOWALKFTlPO"
            };
            var response1 = await _httpClient.PostAsJsonAsync<ApiKeyModel>("/api/auth/get-token/", apiKeyModel);
            var accessToken = await response1.Content.ReadAsStringAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("/api/CustomerInfo/admin");
            
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetCustomers_Unauthorized()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/admin");
            
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

    }
}
