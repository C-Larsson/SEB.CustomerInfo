using CustomerInfo.REST.Data;
using CustomerInfo.REST.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInfo.REST.Services.CustomerInfoServices
{
    public class CustomerInfoService : ICustomerInfoService
    {

        private readonly AppDbContext _dbContext;

        public CustomerInfoService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Customer?> GetBySsn(string ssn)
        {
            return await _dbContext.Customers.FindAsync(ssn);
        }

        public async Task<Customer> Create(Customer customer)
        {
            TransformPhoneIfNeeded(customer);
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> Update(Customer customer)
        {
            TransformPhoneIfNeeded(customer);
            var customerDB = await _dbContext.Customers.FindAsync(customer.SSN);

            // Only update if not null
            if (customer.Email != null) customerDB.Email = customer.Email;
            if (customer.PhoneNumber != null) customerDB.PhoneNumber = customer.PhoneNumber;

            await _dbContext.SaveChangesAsync();
            return customerDB;
        }

        public async Task<bool> Delete(string ssn)
        {
            var customer = await _dbContext.Customers.FindAsync(ssn);
            if (customer != null)
            {
                _dbContext.Customers.Remove(customer);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Customer>> GetUsers()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        private void TransformPhoneIfNeeded(Customer customer)
        {
            // Replace leading 0 with +46
            if (customer.PhoneNumber != null && customer.PhoneNumber.StartsWith("0"))
                customer.PhoneNumber = $"+46{customer.PhoneNumber.Remove(0, 1)}";
        }

    }

}

