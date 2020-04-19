using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class PentaDeca : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Statistics", "Code", "dbo.Codes");
            DropIndex("dbo.Statistics", new[] { "Code" });
            DropPrimaryKey("dbo.Statistics");
            DropPrimaryKey("dbo.ImitationGames");
            AddColumn("dbo.ImitationGames", "Statistics_Assets", c => c.Long());
            AddColumn("dbo.ImitationGames", "Statistics_Code", c => c.String(maxLength: 8));
            AddColumn("dbo.ImitationGames", "Statistics_Commission", c => c.Double());
            AddColumn("dbo.ImitationGames", "Statistics_MarginRate", c => c.Double());
            AddColumn("dbo.ImitationGames", "Statistics_Strategy", c => c.String(maxLength: 128));
            AddColumn("dbo.ImitationGames", "Statistics_RollOver", c => c.Boolean());
            AlterColumn("dbo.Statistics", "Code", c => c.String(nullable: false, maxLength: 8));
            AddPrimaryKey("dbo.Statistics", new[] { "Assets", "Code", "Commission", "MarginRate", "Strategy", "RollOver" });
            AddPrimaryKey("dbo.ImitationGames", new[] { "Date", "BaseShort", "BaseLong", "NonaTime", "NonaShort", "NonaLong", "OctaTime", "OctaShort", "OctaLong", "HeptaTime", "HeptaShort", "HeptaLong", "HexaTime", "HexaShort", "HexaLong", "PentaTime", "PentaShort", "PentaLong", "QuadTime", "QuadShort", "QuadLong", "TriTime", "TriShort", "TriLong", "DuoTime", "DuoShort", "DuoLong", "MonoTime", "MonoShort", "MonoLong" });
            CreateIndex("dbo.Statistics", "Code");
            CreateIndex("dbo.ImitationGames", new[] { "Statistics_Assets", "Statistics_Code", "Statistics_Commission", "Statistics_MarginRate", "Statistics_Strategy", "Statistics_RollOver" });
            AddForeignKey("dbo.ImitationGames", new[] { "Statistics_Assets", "Statistics_Code", "Statistics_Commission", "Statistics_MarginRate", "Statistics_Strategy", "Statistics_RollOver" }, "dbo.Statistics", new[] { "Assets", "Code", "Commission", "MarginRate", "Strategy", "RollOver" });
            AddForeignKey("dbo.Statistics", "Code", "dbo.Codes", "Code", cascadeDelete: true);
        }
        public override void Down()
        {
            DropForeignKey("dbo.Statistics", "Code", "dbo.Codes");
            DropForeignKey("dbo.ImitationGames", new[] { "Statistics_Assets", "Statistics_Code", "Statistics_Commission", "Statistics_MarginRate", "Statistics_Strategy", "Statistics_RollOver" }, "dbo.Statistics");
            DropIndex("dbo.ImitationGames", new[] { "Statistics_Assets", "Statistics_Code", "Statistics_Commission", "Statistics_MarginRate", "Statistics_Strategy", "Statistics_RollOver" });
            DropIndex("dbo.Statistics", new[] { "Code" });
            DropPrimaryKey("dbo.ImitationGames");
            DropPrimaryKey("dbo.Statistics");
            AlterColumn("dbo.Statistics", "Code", c => c.String(maxLength: 8));
            DropColumn("dbo.ImitationGames", "Statistics_RollOver");
            DropColumn("dbo.ImitationGames", "Statistics_Strategy");
            DropColumn("dbo.ImitationGames", "Statistics_MarginRate");
            DropColumn("dbo.ImitationGames", "Statistics_Commission");
            DropColumn("dbo.ImitationGames", "Statistics_Code");
            DropColumn("dbo.ImitationGames", "Statistics_Assets");
            AddPrimaryKey("dbo.ImitationGames", new[] { "BaseShort", "BaseLong", "NonaTime", "NonaShort", "NonaLong", "OctaTime", "OctaShort", "OctaLong", "HeptaTime", "HeptaShort", "HeptaLong", "HexaTime", "HexaShort", "HexaLong", "PentaTime", "PentaShort", "PentaLong", "QuadTime", "QuadShort", "QuadLong", "TriTime", "TriShort", "TriLong", "DuoTime", "DuoShort", "DuoLong", "MonoTime", "MonoShort", "MonoLong", "Date" });
            AddPrimaryKey("dbo.Statistics", new[] { "Assets", "Commission", "MarginRate", "Strategy", "RollOver" });
            CreateIndex("dbo.Statistics", "Code");
            AddForeignKey("dbo.Statistics", "Code", "dbo.Codes", "Code");
        }
    }
}