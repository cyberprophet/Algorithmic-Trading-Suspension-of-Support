using System.Data.Entity;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatContext
{
    sealed class GoblinBatDbContext : DbContext
    {
        internal GoblinBatDbContext(string key) : base(new Secret().GetPort(key))
        {

        }
        internal DbSet<Codes> Codes
        {
            get; set;
        }
        internal DbSet<Futures> Futures
        {
            get; set;
        }
        internal DbSet<Options> Options
        {
            get; set;
        }
        internal DbSet<Stocks> Stocks
        {
            get; set;
        }
        internal DbSet<Days> Days
        {
            get; set;
        }
        internal DbSet<Datum> Datums
        {
            get; set;
        }
        internal DbSet<Logs> Logs
        {
            get; set;
        }
        internal DbSet<Quotes> Quotes
        {
            get; set;
        }
        internal DbSet<Commentary> Commentaries
        {
            get; set;
        }
        public override int SaveChanges() => this.BatchSaveChanges();
    }
}