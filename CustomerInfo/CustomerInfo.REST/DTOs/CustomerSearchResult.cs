using CustomerInfo.REST.Entities;

namespace CustomerInfo.REST.DTOs
{
    public class CustomerSearchResult
    {

        public List<Customer> Customers { get; set; } = new List<Customer>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }

    }
}
