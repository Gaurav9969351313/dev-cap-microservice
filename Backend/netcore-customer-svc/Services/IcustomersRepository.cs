using netcore_customer_svc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netcore_customer_svc.Services
{
    public interface IcustomersRepository
    {
        ICollection<Customer> Getcustomers();
        Customer Getcustomer(int customersId);

        bool customersExists(int customersId);

        bool Createcustomers(Customer customers);
        bool Updatecustomers(Customer customers);
        bool Deletecustomers(Customer customers);
        bool Save();
    }
}