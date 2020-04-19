using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class TriDeca : DbMigration
    {
        public override void Up()
        {
            CreateTable("dbo.Charts", c => new
            {
                Code = c.String(nullable: false, maxLength: 8),
                Time = c.Int(nullable: false),
                Short = c.Int(nullable: false),
                Long = c.Int(nullable: false),
                Date = c.String(nullable: false, maxLength: 6),
                ShortValue = c.Double(nullable: false),
                LongValue = c.Double(nullable: false),
            }).PrimaryKey(t => new { t.Code, t.Time, t.Short, t.Long, t.Date }).ForeignKey("dbo.Codes", t => t.Code, cascadeDelete: true).Index(t => t.Code);
            CreateTable("dbo.ImitationGames", c => new
            {
                Strategy = c.String(nullable: false, maxLength: 128),
                RollOver = c.Boolean(nullable: false),
                BaseShort = c.Int(nullable: false),
                BaseLong = c.Int(nullable: false),
                NonaTime = c.Int(nullable: false),
                NonaShort = c.Int(nullable: false),
                NonaLong = c.Int(nullable: false),
                OctaTime = c.Int(nullable: false),
                OctaShort = c.Int(nullable: false),
                OctaLong = c.Int(nullable: false),
                HeptaTime = c.Int(nullable: false),
                HeptaShort = c.Int(nullable: false),
                HeptaLong = c.Int(nullable: false),
                HexaTime = c.Int(nullable: false),
                HexaShort = c.Int(nullable: false),
                HexaLong = c.Int(nullable: false),
                PentaTime = c.Int(nullable: false),
                PentaShort = c.Int(nullable: false),
                PentaLong = c.Int(nullable: false),
                QuadTime = c.Int(nullable: false),
                QuadShort = c.Int(nullable: false),
                QuadLong = c.Int(nullable: false),
                TriTime = c.Int(nullable: false),
                TriShort = c.Int(nullable: false),
                TriLong = c.Int(nullable: false),
                DuoTime = c.Int(nullable: false),
                DuoShort = c.Int(nullable: false),
                DuoLong = c.Int(nullable: false),
                MonoTime = c.Int(nullable: false),
                MonoShort = c.Int(nullable: false),
                MonoLong = c.Int(nullable: false),
                Date = c.String(nullable: false, maxLength: 6),
                MarginRate = c.Double(nullable: false),
                BaseTime = c.Int(nullable: false),
                Code = c.String(maxLength: 8),
                Assets = c.Long(nullable: false),
                Commission = c.Double(nullable: false),
                Unrealized = c.Long(nullable: false),
                Revenue = c.Long(nullable: false),
                Fees = c.Int(nullable: false),
                Cumulative = c.Long(nullable: false),
                Statistic = c.Int(nullable: false),
            }).PrimaryKey(t => new { t.Strategy, t.RollOver, t.BaseShort, t.BaseLong, t.NonaTime, t.NonaShort, t.NonaLong, t.OctaTime, t.OctaShort, t.OctaLong, t.HeptaTime, t.HeptaShort, t.HeptaLong, t.HexaTime, t.HexaShort, t.HexaLong, t.PentaTime, t.PentaShort, t.PentaLong, t.QuadTime, t.QuadShort, t.QuadLong, t.TriTime, t.TriShort, t.TriLong, t.DuoTime, t.DuoShort, t.DuoLong, t.MonoTime, t.MonoShort, t.MonoLong, t.Date }).ForeignKey("dbo.Codes", t => t.Code).Index(t => t.Code);
            CreateTable("dbo.Identifies", c => new
            {
                Identity = c.String(nullable: false, maxLength: 20),
                Date = c.String(nullable: false, maxLength: 6),
                Code = c.String(maxLength: 8),
                Assets = c.Long(nullable: false),
                Commission = c.Double(nullable: false),
                Strategy = c.String(nullable: false),
                RollOver = c.Int(nullable: false),
            }).PrimaryKey(t => new { t.Identity, t.Date });
        }
        public override void Down()
        {
            DropForeignKey("dbo.Charts", "Code", "dbo.Codes");
            DropForeignKey("dbo.ImitationGames", "Code", "dbo.Codes");
            DropIndex("dbo.ImitationGames", new[] { "Code" });
            DropIndex("dbo.Charts", new[] { "Code" });
            DropTable("dbo.Identifies");
            DropTable("dbo.ImitationGames");
            DropTable("dbo.Charts");
        }
    }
}