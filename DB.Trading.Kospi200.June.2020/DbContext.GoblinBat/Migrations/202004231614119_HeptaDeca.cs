using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class HeptaDeca : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Charts");
            AddColumn("dbo.Charts", "Base", c => c.Int(nullable: false));
            AddColumn("dbo.Charts", "Value", c => c.Double(nullable: false));
            AlterColumn("dbo.Charts", "Date", c => c.String(nullable: false, maxLength: 10));
            AddPrimaryKey("dbo.Charts", new[] { "Code", "Time", "Base", "Date" });
            DropColumn("dbo.Charts", "Short");
            DropColumn("dbo.Charts", "Long");
            DropColumn("dbo.Charts", "ShortValue");
            DropColumn("dbo.Charts", "LongValue");
        }
        public override void Down()
        {
            AddColumn("dbo.Charts", "LongValue", c => c.Double(nullable: false));
            AddColumn("dbo.Charts", "ShortValue", c => c.Double(nullable: false));
            AddColumn("dbo.Charts", "Long", c => c.Int(nullable: false));
            AddColumn("dbo.Charts", "Short", c => c.Int(nullable: false));
            DropPrimaryKey("dbo.Charts");
            AlterColumn("dbo.Charts", "Date", c => c.String(nullable: false, maxLength: 6));
            DropColumn("dbo.Charts", "Value");
            DropColumn("dbo.Charts", "Base");
            AddPrimaryKey("dbo.Charts", new[] { "Code", "Time", "Short", "Long", "Date" });
        }
    }
}