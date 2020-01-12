namespace ShareInvest.GoblinBatContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Codes",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 8),
                        Name = c.String(nullable: false, maxLength: 14),
                        Info = c.String(nullable: false, maxLength: 8),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "dbo.Futures",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 8),
                        Date = c.Long(nullable: false),
                        Price = c.Double(nullable: false),
                        Volume = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Code, t.Date })
                .ForeignKey("dbo.Codes", t => t.Code, cascadeDelete: true)
                .Index(t => t.Code);
            
            CreateTable(
                "dbo.Options",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 8),
                        Date = c.Long(nullable: false),
                        Price = c.Double(nullable: false),
                        Volume = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Code, t.Date })
                .ForeignKey("dbo.Codes", t => t.Code, cascadeDelete: true)
                .Index(t => t.Code);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 8),
                        Date = c.Long(nullable: false),
                        Price = c.Int(nullable: false),
                        Volume = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Code, t.Date })
                .ForeignKey("dbo.Codes", t => t.Code, cascadeDelete: true)
                .Index(t => t.Code);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stocks", "Code", "dbo.Codes");
            DropForeignKey("dbo.Options", "Code", "dbo.Codes");
            DropForeignKey("dbo.Futures", "Code", "dbo.Codes");
            DropIndex("dbo.Stocks", new[] { "Code" });
            DropIndex("dbo.Options", new[] { "Code" });
            DropIndex("dbo.Futures", new[] { "Code" });
            DropTable("dbo.Stocks");
            DropTable("dbo.Options");
            DropTable("dbo.Futures");
            DropTable("dbo.Codes");
        }
    }
}
