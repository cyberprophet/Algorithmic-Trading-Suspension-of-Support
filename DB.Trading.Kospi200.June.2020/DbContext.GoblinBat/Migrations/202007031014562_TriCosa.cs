using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class TriCosa : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Strategics");
            AlterColumn("dbo.Strategics", "Strategy", c => c.String(nullable: false, maxLength: 2));
            AlterColumn("dbo.Strategics", "Primary", c => c.String(nullable: false, maxLength: 20));
            AddPrimaryKey("dbo.Strategics", new[] { "Date", "Strategy", "BaseShort", "BaseLong", "MonoTime", "MonoShort", "MonoLong", "Primary" });
        }
        public override void Down()
        {
            DropPrimaryKey("dbo.Strategics");
            AlterColumn("dbo.Strategics", "Primary", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Strategics", "Strategy", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Strategics", new[] { "Date", "BaseShort", "BaseLong", "NonaTime", "NonaShort", "NonaLong", "OctaTime", "OctaShort", "OctaLong", "HeptaTime", "HeptaShort", "HeptaLong", "HexaTime", "HexaShort", "HexaLong", "PentaTime", "PentaShort", "PentaLong", "QuadTime", "QuadShort", "QuadLong", "TriTime", "TriShort", "TriLong", "DuoTime", "DuoShort", "DuoLong", "MonoTime", "MonoShort", "MonoLong", "Strategy", "Primary" });
        }
    }
}