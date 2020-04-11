using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class Octa : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Commentaries", "Code", "dbo.Codes");
            DropForeignKey("dbo.Logs", "Code", "dbo.Codes");
            DropForeignKey("dbo.Quotes", "Code", "dbo.Codes");
            DropIndex("dbo.Commentaries", new[] { "Code" });
            DropIndex("dbo.Logs", new[] { "Code" });
            DropIndex("dbo.Quotes", new[] { "Code" });
            CreateTable("dbo.Memorizes", c => new
            {
                Index = c.Long(nullable: false),
                Date = c.String(nullable: false, maxLength: 6),
                Code = c.String(nullable: false, maxLength: 8),
                Unrealized = c.String(),
                Revenue = c.String(),
                Commission = c.String(),
                Cumulative = c.String(nullable: false),
                Statistic = c.Int(nullable: false),
            }).PrimaryKey(t => new { t.Index, t.Date, t.Code }).ForeignKey("dbo.Codes", t => t.Code, cascadeDelete: true).Index(t => t.Code);
            DropTable("dbo.Commentaries");
            DropTable("dbo.Logs");
            DropTable("dbo.Quotes");
        }
        public override void Down()
        {
            CreateTable("dbo.Quotes", c => new
            {
                Code = c.String(nullable: false, maxLength: 8),
                Date = c.String(nullable: false, maxLength: 15),
                Contents = c.String(nullable: false),
            }).PrimaryKey(t => new { t.Code, t.Date });
            CreateTable("dbo.Logs", c => new
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
            }).PrimaryKey(t => new { t.Code, t.Strategy, t.Assets, t.Time, t.Short, t.Long, t.Date });
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
            }).PrimaryKey(t => new { t.Code, t.Assets, t.Date, t.Strategy });
            DropForeignKey("dbo.Memorizes", "Code", "dbo.Codes");
            DropIndex("dbo.Memorizes", new[] { "Code" });
            DropTable("dbo.Memorizes");
            CreateIndex("dbo.Quotes", "Code");
            CreateIndex("dbo.Logs", "Code");
            CreateIndex("dbo.Commentaries", "Code");
            AddForeignKey("dbo.Quotes", "Code", "dbo.Codes", "Code", cascadeDelete: true);
            AddForeignKey("dbo.Logs", "Code", "dbo.Codes", "Code", cascadeDelete: true);
            AddForeignKey("dbo.Commentaries", "Code", "dbo.Codes", "Code", cascadeDelete: true);
        }
    }
}