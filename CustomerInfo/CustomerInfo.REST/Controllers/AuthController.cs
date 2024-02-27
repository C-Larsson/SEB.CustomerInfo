using CustomerInfo.REST.Models;
using CustomerInfo.REST.Services.ApiKeyServices;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CustomerInfo.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IApiKeyService _apiKeyService;
        
        public AuthController(IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }


        [HttpPost("get-token")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        public ActionResult<string> GetToken(ApiKeyModel apiKeyModel)
        {
            if (_apiKeyService.ValidateKey(apiKeyModel.ApiKey, out string role))
            {
                var token = _apiKeyService.GenerateJwtToken(role);
                return Ok(token);
            }

            return Problem("Key not valid", statusCode: 401);
        }


    }
}
