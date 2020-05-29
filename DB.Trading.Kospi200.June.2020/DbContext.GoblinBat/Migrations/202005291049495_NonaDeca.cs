using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class NonaDeca : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Identifies", "BaseShort", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "BaseLong", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "NonaTime", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "NonaShort", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "NonaLong", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "OctaTime", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "OctaShort", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "OctaLong", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "HeptaTime", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "HeptaShort", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "HeptaLong", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "HexaTime", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "HexaShort", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "HexaLong", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "PentaTime", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "PentaShort", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "PentaLong", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "QuadTime", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "QuadShort", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "QuadLong", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "TriTime", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "TriShort", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "TriLong", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "DuoTime", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "DuoShort", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "DuoLong", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "MonoTime", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "MonoShort", c => c.Int(nullable: false));
            AddColumn("dbo.Identifies", "MonoLong", c => c.Int(nullable: false));
            AlterColumn("dbo.Identifies", "Code", c => c.String(nullable: false, maxLength: 8));
        }
        public override void Down()
        {
            AlterColumn("dbo.Identifies", "Code", c => c.String(maxLength: 8));
            DropColumn("dbo.Identifies", "MonoLong");
            DropColumn("dbo.Identifies", "MonoShort");
            DropColumn("dbo.Identifies", "MonoTime");
            DropColumn("dbo.Identifies", "DuoLong");
            DropColumn("dbo.Identifies", "DuoShort");
            DropColumn("dbo.Identifies", "DuoTime");
            DropColumn("dbo.Identifies", "TriLong");
            DropColumn("dbo.Identifies", "TriShort");
            DropColumn("dbo.Identifies", "TriTime");
            DropColumn("dbo.Identifies", "QuadLong");
            DropColumn("dbo.Identifies", "QuadShort");
            DropColumn("dbo.Identifies", "QuadTime");
            DropColumn("dbo.Identifies", "PentaLong");
            DropColumn("dbo.Identifies", "PentaShort");
            DropColumn("dbo.Identifies", "PentaTime");
            DropColumn("dbo.Identifies", "HexaLong");
            DropColumn("dbo.Identifies", "HexaShort");
            DropColumn("dbo.Identifies", "HexaTime");
            DropColumn("dbo.Identifies", "HeptaLong");
            DropColumn("dbo.Identifies", "HeptaShort");
            DropColumn("dbo.Identifies", "HeptaTime");
            DropColumn("dbo.Identifies", "OctaLong");
            DropColumn("dbo.Identifies", "OctaShort");
            DropColumn("dbo.Identifies", "OctaTime");
            DropColumn("dbo.Identifies", "NonaLong");
            DropColumn("dbo.Identifies", "NonaShort");
            DropColumn("dbo.Identifies", "NonaTime");
            DropColumn("dbo.Identifies", "BaseLong");
            DropColumn("dbo.Identifies", "BaseShort");
        }
    }
}