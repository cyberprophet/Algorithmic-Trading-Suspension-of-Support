using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class Hepta : DbMigration
    {
        public override void Up()
        {
            CreateTable("dbo.Data", c => new
            {
                Code = c.String(nullable: false, maxLength: 8),
                Date = c.String(nullable: false, maxLength: 15),
                Price = c.String(),
                Volume = c.String(),
                SellPrice = c.String(),
                SellQuantity = c.String(),
                TotalSellAmount = c.String(),
                BuyPrice = c.String(),
                BuyQuantity = c.String(),
                TotalBuyAmount = c.String(),
            }).PrimaryKey(t => new { t.Code, t.Date }).ForeignKey("dbo.Codes", t => t.Code, cascadeDelete: true).Index(t => t.Code);
        }
        public override void Down()
        {
            DropForeignKey("dbo.Data", "Code", "dbo.Codes");
            DropIndex("dbo.Data", new[] { "Code" });
            DropTable("dbo.Data");
        }
    }
}