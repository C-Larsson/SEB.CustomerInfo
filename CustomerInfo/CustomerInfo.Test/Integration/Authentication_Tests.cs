using CustomerInfo.REST.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace CustomerInfo.Test.Integration
{
    public class Authentication_Tests : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        public Authentication_Tests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task GetToken_OK()
        {
            // Prepare
            var apiKey = new ApiKeyDto()
            {
                // This is the key for the admin role in appsettings.json
                ApiKey = "9m6KY9Q9aLFGiVsYeaSkQXu6hlgMnWrojzeOSuRiXvQOAHkvEfS9AmzWRadLOP9m" 
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync<ApiKeyDto>("/api/auth/get-token/", apiKey);
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(new JwtSecurityTokenHandler().ReadJwtToken(result)); // Check if the token is a valid JWT
        }

        [Fact]
        public async Task GetToken_Unauthorized()
        {
            // Prepare
            var apiKey = new ApiKeyDto()
            {
                ApiKey = "405e34f0-3235-4507-a153-6c75561c751a" // Invalid key
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync<ApiKeyDto>("/api/auth/get-token/", apiKey);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


    }

}
