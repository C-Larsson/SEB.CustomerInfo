using CustomerInfo.REST.Models;

namespace CustomerInfo.REST.Services.CustomerInfoServices
{
    public interface ICustomerInfoService
    {
        Task<Customer?> GetBySsn(string ssn);
        Task<List<Customer>> GetUsers();
        Task<Customer> Create(Customer customerInfo);
        Task<Customer> Update(Customer customerInfo);
        Task<bool> Delete(string ssn);
        Task<CustomerSearchResult> SearchCustomers(string searchText, int page);
        Task<List<string>> GetCustomerSearchSuggestions(string searchText);

    }
}
