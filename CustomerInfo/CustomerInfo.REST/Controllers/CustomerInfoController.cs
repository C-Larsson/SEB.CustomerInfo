using CustomerInfo.REST.Validation;
using CustomerInfo.REST.Services.CustomerInfoServices;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomerInfo.REST.DTOs;
using CustomerInfo.REST.Entities;
using CustomerInfo.REST.Utilities;

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
        public async Task<ActionResult<Customer>> Put(CustomerDto customerDto)
        {
            if (await _customerInfoService.GetBySsn(customerDto.SSN) == null)
                return Problem("Customer does not exist", statusCode: 404);

            var customer = customerDto.Adapt<Customer>();
            var customerDB = await _customerInfoService.Update(customer);
            return Ok(customerDB);
        }

        /*
        [HttpPatch, Authorize(Roles = "User, Admin")]
        [ProducesResponseType<Customer>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Customer>> Patch(Customer customer)
        {
            if (await _customerInfoService.GetBySsn(customer.SSN) == null)
                return Problem("Customer does not exist", statusCode: 404);

            var customerDB = await _customerInfoService.Update(customer);
            return Ok(customerDB);
        }
        */

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
        [HttpGet("all"), Authorize(Roles = "User, Admin")]
        [ProducesResponseType<CustomerDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerDto>>> GetUsers()
        {
            var customers = await _customerInfoService.GetUsers();
            var customerDTOs = customers.Adapt<List<CustomerDto>>();
            return Ok(customerDTOs);
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

        // Create a test customer
        [HttpPost("test/customer")]
        [ProducesResponseType<Customer>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        public ActionResult<Customer> CreateTestCustomer()
        {
            var testCustomer = _customerInfoService.GenerateTestCustomer();
            return Created("Test customer was created", testCustomer);
        }


    }
}
