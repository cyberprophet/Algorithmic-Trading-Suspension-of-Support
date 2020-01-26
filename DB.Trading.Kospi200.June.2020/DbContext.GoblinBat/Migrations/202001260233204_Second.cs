namespace ShareInvest.GoblinBatContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Second : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 8),
                        Strategy = c.String(nullable: false, maxLength: 128),
                        Assets = c.Long(nullable: false),
                        Time = c.Int(nullable: false),
                        Short = c.Int(nullable: false),
                        Long = c.Int(nullable: false),
                        Date = c.Int(nullable: false),
                        Unrealized = c.Long(nullable: false),
                        Revenue = c.Long(nullable: false),
                        Cumulative = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Code, t.Strategy, t.Assets, t.Time, t.Short, t.Long })
                .ForeignKey("dbo.Codes", t => t.Code, cascadeDelete: true)
                .Index(t => t.Code);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Logs", "Code", "dbo.Codes");
            DropIndex("dbo.Logs", new[] { "Code" });
            DropTable("dbo.Logs");
        }
    }
}