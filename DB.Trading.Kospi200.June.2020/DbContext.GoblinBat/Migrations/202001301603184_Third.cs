namespace ShareInvest.GoblinBatContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Third : DbMigration
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
