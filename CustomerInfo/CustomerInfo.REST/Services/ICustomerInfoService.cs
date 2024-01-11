using CustomerInfo.REST.Models;

namespace CustomerInfo.REST.Services
{
    public interface ICustomerInfoService
    {
        Task<Customer?> GetBySsn(string ssn);
        Task<Customer> Create(Models.Customer customerInfo);
        Task<Customer> Update(Models.Customer customerInfo);
        Task<bool> Delete(string ssn);

    }
}
