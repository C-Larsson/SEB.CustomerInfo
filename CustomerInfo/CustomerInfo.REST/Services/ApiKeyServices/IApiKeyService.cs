using CustomerInfo.REST.Models;

namespace CustomerInfo.REST.Services.ApiKeyServices
{
    public interface IApiKeyService
    {
        bool ValidateKey(string key, out string role);
        string GenerateJwtToken(string role);

    }
}
