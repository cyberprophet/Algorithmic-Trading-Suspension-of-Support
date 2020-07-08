using Microsoft.EntityFrameworkCore;

using ShareInvest.Models;

namespace ShareInvest.CoreAPI
{
    public class CoreApiDbContext : DbContext
    {
        public CoreApiDbContext(DbContextOptions<CoreApiDbContext> options) : base(options)
        {

        }
        public DbSet<Privacy> Privacies
        {
            get; set;
        }
    }
}