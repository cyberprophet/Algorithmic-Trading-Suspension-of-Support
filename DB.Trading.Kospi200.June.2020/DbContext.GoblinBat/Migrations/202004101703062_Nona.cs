using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class Nona : DbMigration
    {
        public override void Up()
        {
            CreateTable("dbo.Strategics", c => new
            {
                Index = c.Long(nullable: false, identity: true),
                Code = c.String(nullable: false, maxLength: 8),
                Strategy = c.String(nullable: false),
                Assets = c.String(nullable: false),
                Commission = c.String(nullable: false, maxLength: 2),
                MarginRate = c.String(nullable: false),
                RollOver = c.String(nullable: false, maxLength: 1),
                BaseTime = c.String(nullable: false, maxLength: 4),
                BaseShort = c.String(nullable: false),
                BaseLong = c.String(nullable: false),
                NonaTime = c.String(nullable: false),
                NonaShort = c.String(nullable: false),
                NonaLong = c.String(nullable: false),
                OctaTime = c.String(nullable: false),
                OctaShort = c.String(nullable: false),
                OctaLong = c.String(nullable: false),
                HeptaTime = c.String(nullable: false),
                HeptaShort = c.String(nullable: false),
                HeptaLong = c.String(nullable: false),
                HexaTime = c.String(nullable: false),
                HexaShort = c.String(nullable: false),
                HexaLong = c.String(nullable: false),
                PentaTime = c.String(nullable: false),
                PantaShort = c.String(nullable: false),
                PantaLong = c.String(nullable: false),
                QuadTime = c.String(nullable: false),
                QuadShort = c.String(nullable: false),
                QuadLong = c.String(nullable: false),
                TriTime = c.String(nullable: false),
                TriShort = c.String(nullable: false),
                TriLong = c.String(nullable: false),
                DuoTime = c.String(nullable: false),
                DuoShort = c.String(nullable: false),
                DuoLong = c.String(nullable: false),
                MonoTime = c.String(nullable: false),
                MonoShort = c.String(nullable: false),
                MonoLong = c.String(nullable: false),
            }).PrimaryKey(t => t.Index);
            CreateIndex("dbo.Memorizes", "Index");
            AddForeignKey("dbo.Memorizes", "Index", "dbo.Strategics", "Index", cascadeDelete: true);
        }
        public override void Down()
        {
            DropForeignKey("dbo.Memorizes", "Index", "dbo.Strategics");
            DropIndex("dbo.Memorizes", new[] { "Index" });
            DropTable("dbo.Strategics");
        }
    }
}