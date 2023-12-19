using CustomerInfo.REST.Models;
using CustomerInfo.REST.Services;
using CustomerInfo.REST.Utility;
using Microsoft.AspNetCore.Mvc;

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

        // Get customer by ssn
        [HttpGet("{ssn}")]
        public ActionResult<Customer> Get(string ssn)
        {
            if (!SsnValidator.IsValid(ssn))
                return BadRequest("Invalid format of Swedish SSN");

            var customer = _customerInfoService.GetBySsn(ssn);

            if (customer == null)
                return NotFound("Customer not found");

            return Ok(customer);
        }

        // Create a new customer
        [HttpPost]
        public ActionResult<Customer> Post(Customer customer)
        {
            if (!SsnValidator.IsValid(customer.SSN))
                return BadRequest("Invalid format of Swedish SSN");

            if (!PhoneNumberValidator.IsValid(customer.PhoneNumber))
                return BadRequest("Invalid format of Swedish phone number");

            if (_customerInfoService.GetBySsn(customer.SSN) != null)
                return Conflict("Customer already exists");
 
            var customerDB = _customerInfoService.Create(customer);
            return Created("Customer was created", customerDB);
        }

        // Update a customer
        [HttpPut]
        public ActionResult<Customer> Put(Customer customer)
        {
            if (!SsnValidator.IsValid(customer.SSN))
                return BadRequest("Invalid format for Swedish SSN");

            if (!PhoneNumberValidator.IsValid(customer.PhoneNumber))
                return BadRequest("Invalid format of Swedish phone number");

            if (_customerInfoService.GetBySsn(customer.SSN) == null)
                return NotFound("Customer does not exist");

            var customerDB = _customerInfoService.Update(customer);
            return Ok(customerDB);
        }

        // Delete a customer
        [HttpDelete("{ssn}")]
        public IActionResult Delete(string ssn)
        {
            bool result = _customerInfoService.Delete(ssn);

            if (!result)
                return NotFound("Customer was not found");
  
            return Ok("Customer was deleted");
        }

    }
}
