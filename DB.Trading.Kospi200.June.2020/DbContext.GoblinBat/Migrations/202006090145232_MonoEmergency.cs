using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class MonoEmergency : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Memorizes", "Code", "dbo.Codes");
            DropForeignKey("dbo.Memorizes", "Index", "dbo.Strategics");
            DropIndex("dbo.Memorizes", new[] { "Index" });
            DropIndex("dbo.Memorizes", new[] { "Code" });
            DropTable("dbo.Logs");
            DropTable("dbo.Memorizes");
            DropTable("dbo.Strategics");
        }
        public override void Down()
        {
            CreateTable("dbo.Strategics", c => new
            {
                Index = c.Long(nullable: false, identity: true),
                Code = c.String(nullable: false, maxLength: 6),
                Strategy = c.String(nullable: false, maxLength: 4),
                Assets = c.String(nullable: false, maxLength: 4),
                Commission = c.String(nullable: false, maxLength: 2),
                MarginRate = c.String(nullable: false),
                RollOver = c.String(nullable: false, maxLength: 1),
                BaseTime = c.String(nullable: false, maxLength: 4),
                BaseShort = c.String(nullable: false, maxLength: 4),
                BaseLong = c.String(nullable: false, maxLength: 4),
                NonaTime = c.String(nullable: false, maxLength: 4),
                NonaShort = c.String(nullable: false, maxLength: 4),
                NonaLong = c.String(nullable: false, maxLength: 4),
                OctaTime = c.String(nullable: false, maxLength: 4),
                OctaShort = c.String(nullable: false, maxLength: 4),
                OctaLong = c.String(nullable: false, maxLength: 4),
                HeptaTime = c.String(nullable: false, maxLength: 4),
                HeptaShort = c.String(nullable: false, maxLength: 4),
                HeptaLong = c.String(nullable: false, maxLength: 4),
                HexaTime = c.String(nullable: false, maxLength: 4),
                HexaShort = c.String(nullable: false, maxLength: 4),
                HexaLong = c.String(nullable: false, maxLength: 4),
                PentaTime = c.String(nullable: false, maxLength: 4),
                PantaShort = c.String(nullable: false, maxLength: 4),
                PantaLong = c.String(nullable: false, maxLength: 4),
                QuadTime = c.String(nullable: false, maxLength: 4),
                QuadShort = c.String(nullable: false, maxLength: 4),
                QuadLong = c.String(nullable: false, maxLength: 4),
                TriTime = c.String(nullable: false, maxLength: 4),
                TriShort = c.String(nullable: false, maxLength: 4),
                TriLong = c.String(nullable: false, maxLength: 4),
                DuoTime = c.String(nullable: false, maxLength: 4),
                DuoShort = c.String(nullable: false, maxLength: 4),
                DuoLong = c.String(nullable: false, maxLength: 4),
                MonoTime = c.String(nullable: false, maxLength: 4),
                MonoShort = c.String(nullable: false, maxLength: 4),
                MonoLong = c.String(nullable: false, maxLength: 4),
            }).PrimaryKey(t => t.Index);
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
            }).PrimaryKey(t => new { t.Index, t.Date, t.Code });
            CreateTable("dbo.Logs", c => new
            {
                Identity = c.String(nullable: false, maxLength: 20),
                Date = c.String(nullable: false, maxLength: 6),
                Assets = c.String(nullable: false, maxLength: 4),
                Strategy = c.String(nullable: false, maxLength: 4),
                Commission = c.String(nullable: false, maxLength: 2),
                RollOver = c.String(nullable: false, maxLength: 1),
                Code = c.String(nullable: false, maxLength: 6),
            }).PrimaryKey(t => new { t.Identity, t.Date });
            CreateIndex("dbo.Memorizes", "Code");
            CreateIndex("dbo.Memorizes", "Index");
            AddForeignKey("dbo.Memorizes", "Index", "dbo.Strategics", "Index", cascadeDelete: true);
            AddForeignKey("dbo.Memorizes", "Code", "dbo.Codes", "Code", cascadeDelete: true);
        }
    }
}