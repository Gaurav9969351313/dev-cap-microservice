using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using netcore_customer_svc.Models;

namespace netcore_customer_svc.Services
{
    public class GenericDbContext : DbContext
    {
        public GenericDbContext(DbContextOptions<GenericDbContext> options) : base(options)
        {
            Database.Migrate();
        }
        public virtual DbSet<Customer> Customers { get; set; }
    }
}
