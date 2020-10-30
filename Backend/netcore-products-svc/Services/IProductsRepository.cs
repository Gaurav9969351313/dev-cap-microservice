using netcore_products_svc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netcore_products_svc.Services
{
    public interface IProductsRepository
    {
        ICollection<Product> Getproducts();
        Product Getproduct(int productsId);

        bool productsExists(int productsId);

        bool Createproducts(Product products);
        bool Updateproducts(Product products);
        bool Deleteproducts(Product products);
        bool Save();
    }
}