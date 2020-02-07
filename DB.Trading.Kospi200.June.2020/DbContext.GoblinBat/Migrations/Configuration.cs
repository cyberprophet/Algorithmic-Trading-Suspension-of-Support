using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<GoblinBatDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
        protected override void Seed(GoblinBatDbContext context)
        {

        }
    }
}