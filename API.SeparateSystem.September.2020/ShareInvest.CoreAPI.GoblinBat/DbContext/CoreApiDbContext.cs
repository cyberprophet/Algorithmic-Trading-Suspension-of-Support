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
        public DbSet<Codes> Codes
        {
            get; set;
        }
        public DbSet<Days> Days
        {
            get; set;
        }
        public DbSet<Stocks> Stocks
        {
            get; set;
        }
        public DbSet<Futures> Futures
        {
            get; set;
        }
        public DbSet<Options> Options
        {
            get; set;
        }
    }
}