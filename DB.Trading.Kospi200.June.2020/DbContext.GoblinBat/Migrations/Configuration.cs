namespace ShareInvest.GoblinBatContext.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ShareInvest.GoblinBatContext.GoblinBatDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ShareInvest.GoblinBatContext.GoblinBatDbContext";
        }

        protected override void Seed(ShareInvest.GoblinBatContext.GoblinBatDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
