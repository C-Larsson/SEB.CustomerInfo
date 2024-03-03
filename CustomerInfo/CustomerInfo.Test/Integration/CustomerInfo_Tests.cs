using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CustomerInfo.Test.Integration
{
    public class CustomerInfo_Tests : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;
        
        private readonly string _userApiKey = "tWoNuTGFkErOBxl0RVZRZGs3LZwjCTtjDaJAQsmAwFBznufYj4QHfeOWALKFTlPO";
        private readonly string _adminApiKey = "9m6KY9Q9aLFGiVsYeaSkQXu6hlgMnWrojzeOSuRiXvQOAHkvEfS9AmzWRadLOP9m";
        
        private readonly string _userAccessToken;
        private readonly string _adminAccessToken;

        public CustomerInfo_Tests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
            _userAccessToken = GetAccessToken(_userApiKey).Result;
            _adminAccessToken = GetAccessToken(_adminApiKey).Result;

            SeedDatabase().Wait();
        }

        [Fact]
        public async Task GetCustomer_OK()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);



            var response = await _httpClient.GetAsync("/api/CustomerInfo/197210161238");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var customer = await response.Content.ReadFromJsonAsync<Customer>();
            Assert.NotNull(customer);
            Assert.Equal("197210161238", customer.SSN);
            Assert.Equal("test2@gmail.com", customer.Email);
            Assert.Equal("+46701234568", customer.PhoneNumber);
        }

        [Fact]
        public async Task GetCustomer_NotFound()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/197210161230"); // SSN does not exist
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateCustomer_Admin_Ok()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminAccessToken);

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
        public async Task CreateCustomer_User_Ok()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

            var customer = new Customer()
            {
                SSN = "197210161236",
                PhoneNumber = "0701234568"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var customerInfoResponse = await response.Content.ReadFromJsonAsync<Customer>();
            Assert.NotNull(customerInfoResponse);
            Assert.Equal("197210161236", customerInfoResponse.SSN);
            Assert.Equal("+46701234568", customerInfoResponse.PhoneNumber);
        }

        [Fact]
        public async Task CreateCustomer_Unauthorized()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = null; // Remove access token

            var customer = new Customer()
            {
                SSN = "197210161235",
                PhoneNumber = "0701234567"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }   


        [Fact]
        public async Task CreateCustomer_User_Conflict()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

            var customer = new Customer()
            {
                SSN = "197210161234", // SSN already exists
                PhoneNumber = "0701234567"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task CreateCustomer_User_BadRequest_SSN()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

            var customer = new Customer()
            {
                SSN = "19721016123", // Invalid SSN
                PhoneNumber = "0701234567"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCustomer_User_Ok()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

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
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

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
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

            var customer = new Customer()
            {
                SSN = "197210161230", // SSN does not exist
                PhoneNumber = "0701234567"
            };

            var response = await _httpClient.PutAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCustomer_Admin_NoContent()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminAccessToken);

            var response = await _httpClient.DeleteAsync("/api/CustomerInfo/admin/197210161234");
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCustomer_Admin_NotFound()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminAccessToken);

            var response = await _httpClient.DeleteAsync("/api/CustomerInfo/admin/197210161230"); // SSN does not exist
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task DeleteCustomer_User_Forbidden()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

            var response = await _httpClient.DeleteAsync("/api/CustomerInfo/admin/197210161234");
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        }


        [Fact]
        public async Task GetCustomers_Admin_OK()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminAccessToken);

            var response = await _httpClient.GetAsync("/api/CustomerInfo/admin");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();
            Assert.NotNull(customers);
            Assert.Equal(2, customers.Count);
        }

        [Fact]
        public async Task GetCustomers_User_Forbidden()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);
            var response = await _httpClient.GetAsync("/api/CustomerInfo/admin");
            
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetCustomers_Unauthorized()
        {
            // Remove access token
            _httpClient.DefaultRequestHeaders.Authorization = null;
            var response = await _httpClient.GetAsync("/api/CustomerInfo/admin");
            
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
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
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

            await _httpClient.PostAsJsonAsync("/api/CustomerInfo", new Customer()
            {
                SSN = "197210161234",
                Email = "test@gmail.com",
                PhoneNumber = "+46701234567"
            });

            await _httpClient.PostAsJsonAsync("/api/CustomerInfo", new Customer()
            {
                SSN = "197210161238",
                Email = "test2@gmail.com",
                PhoneNumber = "+46701234568"
            });
        }

    }
}
