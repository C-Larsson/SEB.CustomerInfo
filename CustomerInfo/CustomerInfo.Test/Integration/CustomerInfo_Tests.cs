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



            var response = await _httpClient.GetAsync("/api/CustomerInfo/200001011001");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var customer = await response.Content.ReadFromJsonAsync<Customer>();
            Assert.NotNull(customer);
            Assert.Equal("200001011001", customer.SSN);
            Assert.Equal("test1@gmail.com", customer.Email);
            Assert.Equal("+46720010001", customer.PhoneNumber);
        }

        [Fact]
        public async Task GetCustomer_NotFound()
        {
            var response = await _httpClient.GetAsync("/api/CustomerInfo/200001011020"); // SSN does not exist
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateCustomer_Admin_Ok()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminAccessToken);

            var customer = new Customer()
            {
                SSN = "200001011011",
                PhoneNumber = "0720010011"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var customerInfoResponse = await response.Content.ReadFromJsonAsync<Customer>();
            Assert.NotNull(customerInfoResponse);
            Assert.Equal("200001011011", customerInfoResponse.SSN);
            Assert.Equal("+46720010011", customerInfoResponse.PhoneNumber);
        }

        [Fact]
        public async Task CreateCustomer_User_Ok()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

            var customer = new Customer()
            {
                SSN = "200001011012",
                PhoneNumber = "0720010012"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var customerInfoResponse = await response.Content.ReadFromJsonAsync<Customer>();
            Assert.NotNull(customerInfoResponse);
            Assert.Equal("200001011012", customerInfoResponse.SSN);
            Assert.Equal("+46720010012", customerInfoResponse.PhoneNumber);
        }

        [Fact]
        public async Task CreateCustomer_Unauthorized()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = null; // Remove access token

            var customer = new Customer()
            {
                SSN = "200001011013",
                PhoneNumber = "0720010013"
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
                SSN = "200001011001", // SSN already exists
                PhoneNumber = "0720010001"
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
                SSN = "20000101101499", // Invalid SSN
                PhoneNumber = "0720010014"
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
                SSN = "200001011002",
                PhoneNumber = "0720010022"
            };

            var response = await _httpClient.PutAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var customerInfoResponse = await response.Content.ReadFromJsonAsync<Customer>();
            Assert.NotNull(customerInfoResponse);
            Assert.Equal("200001011002", customerInfoResponse.SSN);
            Assert.Equal("+46720010022", customerInfoResponse.PhoneNumber);
        }

        [Fact]
        public async Task UpdateCustomer_BadRequest_SSN()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

            var customer = new Customer()
            {
                SSN = "20000101100399", // Invalid SSN
                PhoneNumber = "0720010003"
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
                SSN = "200001011030", // SSN does not exist
                PhoneNumber = "0720010030"
            };

            var response = await _httpClient.PutAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCustomer_Admin_NoContent()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminAccessToken);

            var response = await _httpClient.DeleteAsync("/api/CustomerInfo/admin/200001011005");
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCustomer_Admin_NotFound()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminAccessToken);

            var response = await _httpClient.DeleteAsync("/api/CustomerInfo/admin/200001011030"); // SSN does not exist
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task DeleteCustomer_User_Forbidden()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

            var response = await _httpClient.DeleteAsync("/api/CustomerInfo/admin/200001011004");
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

        }

    }
}
