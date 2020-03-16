using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class Hexa : DbMigration
    {
        public override void Up()
        {
            CreateTable("dbo.Commentaries", c => new
            {
                Code = c.String(nullable: false, maxLength: 8),
                Assets = c.Long(nullable: false),
                Date = c.String(nullable: false, maxLength: 6),
                Strategy = c.String(nullable: false, maxLength: 128),
                Unrealized = c.Long(nullable: false),
                Revenue = c.Long(nullable: false),
                Cumulative = c.Long(nullable: false),
                Commission = c.Long(nullable: false),
            }).PrimaryKey(t => new { t.Code, t.Assets, t.Date, t.Strategy }).ForeignKey("dbo.Codes", t => t.Code, cascadeDelete: true).Index(t => t.Code);
        }
        public override void Down()
        {
            DropForeignKey("dbo.Commentaries", "Code", "dbo.Codes");
            DropIndex("dbo.Commentaries", new[] { "Code" });
            DropTable("dbo.Commentaries");
        }
    }
}