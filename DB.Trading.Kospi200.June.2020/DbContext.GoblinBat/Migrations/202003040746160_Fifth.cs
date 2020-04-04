using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    partial class Fifth : DbMigration
    {
        public override void Up()
        {
            CreateTable("dbo.Quotes", c => new
            {
                Code = c.String(nullable: false, maxLength: 8),
                Date = c.String(nullable: false, maxLength: 15),
                Contents = c.String(nullable: false)
            }).PrimaryKey(t => new { t.Code, t.Date }).ForeignKey("dbo.Codes", t => t.Code, cascadeDelete: true).Index(t => t.Code);
        }
        public override void Down()
        {
            DropForeignKey("dbo.Quotes", "Code", "dbo.Codes");
            DropIndex("dbo.Quotes", new[] { "Code" });
            DropTable("dbo.Quotes");
        }
    }
}