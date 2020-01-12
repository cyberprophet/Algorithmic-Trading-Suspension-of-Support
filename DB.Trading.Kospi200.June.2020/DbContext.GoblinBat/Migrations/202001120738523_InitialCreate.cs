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
                        Price = c.Double(nullable: false),
                        Volume = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "dbo.Options",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 8),
                        Price = c.Double(nullable: false),
                        Volume = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 6),
                        Volume = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Code);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Stocks");
            DropTable("dbo.Options");
            DropTable("dbo.Futures");
            DropTable("dbo.Codes");
        }
    }
}
