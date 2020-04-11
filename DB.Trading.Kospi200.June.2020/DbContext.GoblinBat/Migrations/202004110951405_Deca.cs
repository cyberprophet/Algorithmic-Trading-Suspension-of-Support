using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class Deca : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Strategics", "Assets", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "BaseShort", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "BaseLong", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "NonaTime", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "NonaShort", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "NonaLong", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "OctaTime", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "OctaShort", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "OctaLong", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "HeptaTime", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "HeptaShort", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "HeptaLong", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "HexaTime", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "HexaShort", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "HexaLong", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "PentaTime", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "PantaShort", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "PantaLong", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "QuadTime", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "QuadShort", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "QuadLong", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "TriTime", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "TriShort", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "TriLong", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "DuoTime", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "DuoShort", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "DuoLong", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "MonoTime", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "MonoShort", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Strategics", "MonoLong", c => c.String(nullable: false, maxLength: 4));
        }
        public override void Down()
        {
            AlterColumn("dbo.Strategics", "MonoLong", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "MonoShort", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "MonoTime", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "DuoLong", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "DuoShort", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "DuoTime", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "TriLong", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "TriShort", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "TriTime", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "QuadLong", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "QuadShort", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "QuadTime", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "PantaLong", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "PantaShort", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "PentaTime", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "HexaLong", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "HexaShort", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "HexaTime", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "HeptaLong", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "HeptaShort", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "HeptaTime", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "OctaLong", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "OctaShort", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "OctaTime", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "NonaLong", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "NonaShort", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "NonaTime", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "BaseLong", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "BaseShort", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "Assets", c => c.String(nullable: false));
        }
    }
}