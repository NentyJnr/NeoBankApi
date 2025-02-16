using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NeoBank.Models;

namespace NeoBank.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
       
        
    }
}
