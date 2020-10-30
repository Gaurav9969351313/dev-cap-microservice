using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace netcore_auth_svc.Data
{
    public class ApplicationDBContext  : IdentityDbContext
    {
        public ApplicationDBContext()
        { }

        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}