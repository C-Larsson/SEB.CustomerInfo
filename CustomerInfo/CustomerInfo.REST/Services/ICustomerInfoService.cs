using CustomerInfo.REST.Models;

namespace CustomerInfo.REST.Services
{
    public interface ICustomerInfoService
    {
        Customer? GetBySsn(string ssn);
        Customer Create(Models.Customer customerInfo);
        Customer Update(Models.Customer customerInfo);
        bool Delete(string ssn);

    }
}
