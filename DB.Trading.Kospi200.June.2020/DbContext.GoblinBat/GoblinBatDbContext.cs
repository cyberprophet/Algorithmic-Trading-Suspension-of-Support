using System.Data.Entity;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatContext
{
    public class GoblinBatDbContext : DbContext
    {
        public GoblinBatDbContext() : base(new Secret().ConnectionString)
        {

        }
        public DbSet<Codes> Codes
        {
            get; set;
        }
    }
}