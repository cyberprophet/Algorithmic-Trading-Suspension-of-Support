using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class DoCosa : DbMigration
    {
        public override void Up() => CreateTable("dbo.Strategics", c => new
        {
            Date = c.String(maxLength: 6),
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
            Code = c.String(nullable: false, maxLength: 8),
            Assets = c.Long(nullable: false),
            Commission = c.Double(nullable: false),
            RollOver = c.Boolean(nullable: false),
            Strategy = c.String(maxLength: 2),
            MarginRate = c.Double(nullable: false),
            Primary = c.String(maxLength: 20)
        }).PrimaryKey(t => new { t.Date, t.BaseShort, t.BaseLong, t.MonoTime, t.MonoShort, t.MonoLong, t.Strategy, t.Primary }).Index(o => new { o.Date, o.Strategy, o.BaseShort, o.BaseLong, o.MonoTime, o.MonoShort, o.MonoLong, o.Primary });
        public override void Down() => DropTable("dbo.Strategics");
    }
}