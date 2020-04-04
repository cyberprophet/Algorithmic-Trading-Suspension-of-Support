using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    partial class Third : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Logs");
            AddPrimaryKey("dbo.Logs", new[] { "Code", "Strategy", "Assets", "Time", "Short", "Long", "Date" });
        }
        public override void Down()
        {
            DropPrimaryKey("dbo.Logs");
            AddPrimaryKey("dbo.Logs", new[] { "Code", "Strategy", "Assets", "Time", "Short", "Long" });
        }
    }
}