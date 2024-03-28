using CustomerInfo.REST.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using CustomerInfo.REST.DTOs;
using CustomerInfo.REST.Entities;
using CustomerInfo.REST.Utilities;

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
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.SSN.Equals(ssn) && c.IsDeleted == false);
            return customer;
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
            var customerDB = await _dbContext.Customers.FirstOrDefaultAsync(c => c.SSN.Equals(customer.SSN) && c.IsDeleted == false);

            // Only update if not null
            if (customer.Email != null) customerDB.Email = customer.Email;
            if (customer.PhoneNumber != null) customerDB.PhoneNumber = customer.PhoneNumber;

            await _dbContext.SaveChangesAsync();
            return customerDB;
        }

        public async Task<bool> Delete(string ssn)
        {
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.SSN.Equals(ssn) && c.IsDeleted == false);
            if (customer != null)
            {
                customer.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Customer>> GetUsers()
        {
            return await _dbContext.Customers.Where(c => c.IsDeleted == false).ToListAsync();
        }

        public async Task<CustomerSearchResult> SearchCustomers(string searchText, int pageSize, int page)
        {
            var pageCount = Math.Ceiling((await FindCustomersBySearchText(searchText)).Count / (decimal) pageSize);

            var customers = await _dbContext.Customers
                .OrderBy(c => c.SSN)
                .Where(c => (c.SSN.ToLower().Contains(searchText.ToLower()) ||
                    c.Email.ToLower().Contains(searchText.ToLower()) ||
                    c.PhoneNumber.ToLower().Contains(searchText.ToLower())) && 
                    c.IsDeleted == false
                    )
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return new CustomerSearchResult
            {
                Customers = customers,
                Pages = (int)pageCount,
                CurrentPage = page
            };

        }

        public async Task<List<string>> GetCustomerSearchSuggestions(string searchText)
        {
            var customers = await FindCustomersBySearchText(searchText);
            var result = new List<string>();

            foreach (var customer in customers)
            {
                if (customer.SSN.ToLower().Contains(searchText.ToLower()))
                    result.Add(customer.SSN);
                if (customer.Email.ToLower().Contains(searchText.ToLower()))
                    result.Add(customer.Email);
                if (customer.PhoneNumber.ToLower().Contains(searchText.ToLower()))
                    result.Add(customer.PhoneNumber);
            }
            return result;
        }

        public async Task<Customer> GenerateTestCustomer()
        {
            var testCustomer = new TestCustomer();
            var customer = _dbContext.Customers.Add(testCustomer).Entity;
            await _dbContext.SaveChangesAsync();
            return customer;
        }

        private void TransformPhoneIfNeeded(Customer customer)
        {
            // Replace leading 0 with +46
            if (customer.PhoneNumber != null && customer.PhoneNumber.StartsWith("0"))
                customer.PhoneNumber = $"+46{customer.PhoneNumber.Remove(0, 1)}";
        }

        private async Task<List<Customer>> FindCustomersBySearchText(string searchText)
        {
            return await _dbContext.Customers
                .Where(c => (c.SSN.ToLower().Contains(searchText.ToLower()) ||
                    c.Email.ToLower().Contains(searchText.ToLower()) ||
                    c.PhoneNumber.ToLower().Contains(searchText.ToLower())) && 
                    c.IsDeleted == false).ToListAsync();
        }


    }

}

