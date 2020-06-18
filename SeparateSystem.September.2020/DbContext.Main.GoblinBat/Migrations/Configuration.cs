using System.Data.Entity.Migrations;

namespace ShareInvest.Main.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<MainContext>
    {
        protected override void Seed(MainContext context)
        {

        }
        public Configuration() => AutomaticMigrationsEnabled = false;
    }
}