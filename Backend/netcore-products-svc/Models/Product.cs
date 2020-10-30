using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace netcore_products_svc.Models
{
    public class Product
    {

        [Key]
        public int product_id { get; set; }
        public String product_name { get; set; }
        public int brand_id { get; set; }
        public int category_id { get; set; }
        public int model_year { get; set; }
        public float list_price { get; set; }

    }
}