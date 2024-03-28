using CustomerInfo.REST.DTOs;
using CustomerInfo.REST.Entities;

namespace CustomerInfo.REST.Services.CustomerInfoServices
{
    public interface ICustomerInfoService
    {
        Task<Customer?> GetBySsn(string ssn);
        Task<List<Customer>> GetUsers();
        Task<Customer> Create(Customer customer);
        Task<Customer> Update(Customer customer);
        Task<bool> Delete(string ssn);
        Task<CustomerSearchResult> SearchCustomers(string searchText, int pageSize, int page);
        Task<List<string>> GetCustomerSearchSuggestions(string searchText);
        Task<Customer> GenerateTestCustomer();

    }
}
