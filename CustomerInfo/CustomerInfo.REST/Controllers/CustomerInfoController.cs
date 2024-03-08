using CustomerInfo.REST.Models;
using CustomerInfo.REST.Services.CustomerInfoServices;
using CustomerInfo.REST.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data;

namespace CustomerInfo.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerInfoController : ControllerBase
    {
        private readonly ICustomerInfoService _customerInfoService;

        public CustomerInfoController(ICustomerInfoService customerInfoService)
        {
            _customerInfoService = customerInfoService;
        }

        [HttpGet("{ssn}")]
        [ProducesResponseType<Customer>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Customer>> Get([SSN] string ssn)
        {
            var customer = await _customerInfoService.GetBySsn(ssn);

            return (customer == null) ?
            Problem("Customer not found", statusCode: 404) :
            Ok(customer);
        }

        // Create a new customer
        [HttpPost, Authorize(Roles = "User, Admin")]
        [ProducesResponseType<Customer>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Customer>> Post(Customer customer)
        {
            if (await _customerInfoService.GetBySsn(customer.SSN) != null)
                return Problem("Customer already exists", statusCode: 409);

            var customerDB = await _customerInfoService.Create(customer);
            return Created("Customer was created", customerDB);
        }

        // Update a customer
        [HttpPut, Authorize(Roles = "User, Admin")]
        [ProducesResponseType<Customer>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Customer>> Put(Customer customer)
        {
            if (await _customerInfoService.GetBySsn(customer.SSN) == null)
                return Problem("Customer does not exist", statusCode: 404);

            var customerDB = await _customerInfoService.Update(customer);
            return Ok(customerDB);
        }

        // Delete a customer
        [HttpDelete("admin/{ssn}"), Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([SSN] string ssn)
        {
            bool result = await _customerInfoService.Delete(ssn);

            if (!result)
                return Problem("Customer not found", statusCode: 404);

            return NoContent();
        }

        // Get all users
        [HttpGet("admin"), Authorize(Roles = "Admin")]
        [ProducesResponseType<Customer>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Customer>>> GetUsers()
        {
            var result = await _customerInfoService.GetUsers();
            return Ok(result);
        }

        // Search for customers
        [HttpGet("search")]
        [ProducesResponseType<CustomerSearchResult>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerSearchResult>> SearchCustomers([FromQuery] string searchText = "", [FromQuery]  int pageSize = 5, [FromQuery] int page = 1)
        {
            var result = await _customerInfoService.SearchCustomers(searchText, pageSize, page);
            return Ok(result);
        }

        // Get search suggestions
        [HttpGet("search/suggestions/{searchText}")]
        [ProducesResponseType<List<string>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<string>>> GetCustomerSearchSuggestions(string searchText)
        {
            var result = await _customerInfoService.GetCustomerSearchSuggestions(searchText);
            return Ok(result);
        }

    }
}
