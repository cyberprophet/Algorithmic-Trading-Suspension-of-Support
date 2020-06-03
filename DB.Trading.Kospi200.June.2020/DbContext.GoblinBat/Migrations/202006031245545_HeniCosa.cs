using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class HeniCosa : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Games");
        }
        public override void Down()
        {
            CreateTable("dbo.Games", c => new
            {
                Date = c.String(nullable: false, maxLength: 6),
                Unrealized = c.Long(nullable: false),
                Revenue = c.Long(nullable: false),
                Fees = c.Int(nullable: false),
                Cumulative = c.Long(nullable: false),
                Statistic = c.Int(nullable: false),
                BaseTime = c.Int(nullable: false),
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
                Assets = c.Long(nullable: false),
                Code = c.String(maxLength: 8),
                Commission = c.Double(nullable: false),
                MarginRate = c.Double(nullable: false),
                Strategy = c.String(nullable: false, maxLength: 128),
                RollOver = c.Boolean(nullable: false),
            }).PrimaryKey(t => new { t.Date, t.BaseShort, t.BaseLong, t.NonaTime, t.NonaShort, t.NonaLong, t.OctaTime, t.OctaShort, t.OctaLong, t.HeptaTime, t.HeptaShort, t.HeptaLong, t.HexaTime, t.HexaShort, t.HexaLong, t.PentaTime, t.PentaShort, t.PentaLong, t.QuadTime, t.QuadShort, t.QuadLong, t.TriTime, t.TriShort, t.TriLong, t.DuoTime, t.DuoShort, t.DuoLong, t.MonoTime, t.MonoShort, t.MonoLong, t.Strategy, t.RollOver });
        }
    }
}