using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using netcore_customer_svc.Models;
using netcore_customer_svc.Services;

namespace netcore_customer_svc.Services
{
    public class CustomersRepository : IcustomersRepository
    {
        private GenericDbContext _customersContext;

        public CustomersRepository(GenericDbContext customersContext)
        {
            _customersContext = customersContext;
        }

        public bool customersExists(int customersId)
        {
            return _customersContext.Customers.Any(a => a.customer_id == customersId);
        }

        public bool Createcustomers(Customer customers)
        {
            _customersContext.Add(customers);
            return Save();
        }

        public bool Deletecustomers(Customer customers)
        {
            _customersContext.Remove(customers);
            return Save();
        }

        public Customer Getcustomer(int customersId)
        {
            return _customersContext.Customers.Where(a => a.customer_id == customersId).FirstOrDefault();
        }

        public ICollection<Customer> Getcustomers()
        {
            return _customersContext.Customers.OrderBy(a => a.first_name).ToList();
        }

        public bool Save()
        {
            var saved = _customersContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool Updatecustomers(Customer customers)
        {
            _customersContext.Update(customers);
            return Save();
        }
    }
}