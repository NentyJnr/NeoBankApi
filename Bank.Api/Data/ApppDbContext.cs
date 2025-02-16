using Bank.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Data
{
    public class ApppDbContext :DbContext
    {
        public ApppDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Account> Accounts { get; set; }

    }
}
