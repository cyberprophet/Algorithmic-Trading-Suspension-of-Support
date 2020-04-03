using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<GoblinBatDbContext>
    {
        protected override void Seed(GoblinBatDbContext context)
        {

        }
        public Configuration() => AutomaticMigrationsEnabled = false;
    }
}