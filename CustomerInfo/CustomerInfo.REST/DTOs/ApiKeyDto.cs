using System.Security.Cryptography.X509Certificates;

namespace CustomerInfo.REST.DTOs
{
    public class ApiKeyDto
    {
        public required string ApiKey { get; set; }
    }
}
