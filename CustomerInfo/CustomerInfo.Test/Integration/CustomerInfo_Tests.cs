using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CustomerInfo.REST.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerInfo.Test.Integration
{
    [Collection("Sequential")]
    public class CustomerInfo_Tests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {

        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;
        
        private readonly string _userApiKey = "tWoNuTGFkErOBxl0RVZRZGs3LZwjCTtjDaJAQsmAwFBznufYj4QHfeOWALKFTlPO";
        private readonly string _adminApiKey = "9m6KY9Q9aLFGiVsYeaSkQXu6hlgMnWrojzeOSuRiXvQOAHkvEfS9AmzWRadLOP9m";
        
        private readonly string _userAccessToken;
        private readonly string _adminAccessToken;

        public CustomerInfo_Tests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();

            var scope = factory.Services.GetService<IServiceScopeFactory>().CreateScope();
            _dbContext = scope.ServiceProvider.GetService<AppDbContext>();

            SeedDatabase().Wait();
           
            _userAccessToken = GetAccessToken(_userApiKey).Result;
            _adminAccessToken = GetAccessToken(_adminApiKey).Result;
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

            var customer = new CustomerDto()
            {
                SSN = "200001011011",
                Name = "Test11 Testsson",
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
                Name = "Test12 Testsson",
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
                Name = "Test13 Testsson",
                PhoneNumber = "0720010013"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }   


        [Fact]
        public async Task CreateCustomer_Conflict()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

            var customer = new Customer()
            {
                SSN = "200001011001", // SSN already exists
                Name = "Test1 Testsson",
                PhoneNumber = "0720010001"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CustomerInfo", customer);
            Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task CreateCustomer_BadRequest()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

            var customer = new Customer()
            {
                SSN = "20000101101499", // Invalid SSN
                Name = "Test14 Testsson",
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
                Name = "Test2 Testsson",
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
        public async Task UpdateCustomer_BadRequest()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);

            var customer = new CustomerDto()
            {
                SSN = "20000101100399", // Invalid SSN
                Name = "Test3 Testsson",
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

            var customer = new CustomerDto()
            {
                SSN = "200001011030", // SSN does not exist
                Name = "Test30 Testsson",
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
        public async Task DeleteCustomer_NotFound()
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

            var response = await _httpClient.GetAsync("/api/CustomerInfo/all");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();
            Assert.NotNull(customers);
        }

        [Fact]
        public async Task GetCustomers_User_OK()
        {
            // Prepare
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAccessToken);
            var response = await _httpClient.GetAsync("/api/CustomerInfo/all");
            
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetCustomers_Unauthorized()
        {
            // Remove access token
            _httpClient.DefaultRequestHeaders.Authorization = null;
            var response = await _httpClient.GetAsync("/api/CustomerInfo/all");
            
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
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

            await _dbContext.SaveChangesAsync();
        }

    }
}
