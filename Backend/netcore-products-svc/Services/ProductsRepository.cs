using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using netcore_products_svc.Models;
using netcore_products_svc.Services;

namespace netcore_products_svc.Services
{
    public class ProductsRepository : IProductsRepository
    {
        private GenericDbContext _productsContext;

        public ProductsRepository(GenericDbContext productsContext)
        {
            _productsContext = productsContext;
        }

        public bool productsExists(int productsId)
        {
            return _productsContext.Products.Any(a => a.product_id == productsId);
        }

        public bool Createproducts(Product products)
        {
            _productsContext.Add(products);
            return Save();
        }

        public bool Deleteproducts(Product products)
        {
            _productsContext.Remove(products);
            return Save();
        }

        public Product Getproduct(int productsId)
        {
            return _productsContext.Products.Where(a => a.product_id == productsId).FirstOrDefault();
        }

        public ICollection<Product> Getproducts()
        {
            return _productsContext.Products.OrderBy(a => a.product_name).ToList();
        }

        public bool Save()
        {
            var saved = _productsContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool Updateproducts(Product products)
        {
            _productsContext.Update(products);
            return Save();
        }
    }
}