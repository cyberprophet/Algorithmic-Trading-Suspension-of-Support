using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class HexaDeca : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ImitationGames", new[] { "Statistics_Assets", "Statistics_Code", "Statistics_Commission", "Statistics_MarginRate", "Statistics_Strategy", "Statistics_RollOver" });
            RenameColumn(table: "dbo.ImitationGames", name: "Statistics_Assets", newName: "Assets");
            RenameColumn(table: "dbo.ImitationGames", name: "Statistics_Code", newName: "Code");
            RenameColumn(table: "dbo.ImitationGames", name: "Statistics_Commission", newName: "Commission");
            RenameColumn(table: "dbo.ImitationGames", name: "Statistics_MarginRate", newName: "MarginRate");
            RenameColumn(table: "dbo.ImitationGames", name: "Statistics_Strategy", newName: "Strategy");
            RenameColumn(table: "dbo.ImitationGames", name: "Statistics_RollOver", newName: "RollOver");
            DropPrimaryKey("dbo.ImitationGames");
            AlterColumn("dbo.ImitationGames", "Assets", c => c.Long(nullable: false));
            AlterColumn("dbo.ImitationGames", "Commission", c => c.Double(nullable: false));
            AlterColumn("dbo.ImitationGames", "MarginRate", c => c.Double(nullable: false));
            AlterColumn("dbo.ImitationGames", "Strategy", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ImitationGames", "RollOver", c => c.Boolean(nullable: false));
            AddPrimaryKey("dbo.ImitationGames", new[] { "Date", "BaseShort", "BaseLong", "NonaTime", "NonaShort", "NonaLong", "OctaTime", "OctaShort", "OctaLong", "HeptaTime", "HeptaShort", "HeptaLong", "HexaTime", "HexaShort", "HexaLong", "PentaTime", "PentaShort", "PentaLong", "QuadTime", "QuadShort", "QuadLong", "TriTime", "TriShort", "TriLong", "DuoTime", "DuoShort", "DuoLong", "MonoTime", "MonoShort", "MonoLong", "Strategy", "RollOver" });
            CreateIndex("dbo.ImitationGames", new[] { "Assets", "Code", "Commission", "MarginRate", "Strategy", "RollOver" });
        }
        public override void Down()
        {
            DropIndex("dbo.ImitationGames", new[] { "Assets", "Code", "Commission", "MarginRate", "Strategy", "RollOver" });
            DropPrimaryKey("dbo.ImitationGames");
            AlterColumn("dbo.ImitationGames", "RollOver", c => c.Boolean());
            AlterColumn("dbo.ImitationGames", "Strategy", c => c.String(maxLength: 128));
            AlterColumn("dbo.ImitationGames", "MarginRate", c => c.Double());
            AlterColumn("dbo.ImitationGames", "Commission", c => c.Double());
            AlterColumn("dbo.ImitationGames", "Assets", c => c.Long());
            AddPrimaryKey("dbo.ImitationGames", new[] { "Date", "BaseShort", "BaseLong", "NonaTime", "NonaShort", "NonaLong", "OctaTime", "OctaShort", "OctaLong", "HeptaTime", "HeptaShort", "HeptaLong", "HexaTime", "HexaShort", "HexaLong", "PentaTime", "PentaShort", "PentaLong", "QuadTime", "QuadShort", "QuadLong", "TriTime", "TriShort", "TriLong", "DuoTime", "DuoShort", "DuoLong", "MonoTime", "MonoShort", "MonoLong" });
            RenameColumn(table: "dbo.ImitationGames", name: "RollOver", newName: "Statistics_RollOver");
            RenameColumn(table: "dbo.ImitationGames", name: "Strategy", newName: "Statistics_Strategy");
            RenameColumn(table: "dbo.ImitationGames", name: "MarginRate", newName: "Statistics_MarginRate");
            RenameColumn(table: "dbo.ImitationGames", name: "Commission", newName: "Statistics_Commission");
            RenameColumn(table: "dbo.ImitationGames", name: "Code", newName: "Statistics_Code");
            RenameColumn(table: "dbo.ImitationGames", name: "Assets", newName: "Statistics_Assets");
            CreateIndex("dbo.ImitationGames", new[] { "Statistics_Assets", "Statistics_Code", "Statistics_Commission", "Statistics_MarginRate", "Statistics_Strategy", "Statistics_RollOver" });
        }
    }
}