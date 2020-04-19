using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class TetraDeca : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ImitationGames", "Code", "dbo.Codes");
            DropIndex("dbo.ImitationGames", new[] { "Code" });
            DropPrimaryKey("dbo.ImitationGames");
            CreateTable("dbo.Statistics", c => new
            {
                Assets = c.Long(nullable: false),
                Code = c.String(maxLength: 8),
                Commission = c.Double(nullable: false),
                MarginRate = c.Double(nullable: false),
                Strategy = c.String(nullable: false, maxLength: 128),
                RollOver = c.Boolean(nullable: false),
            }).PrimaryKey(t => new { t.Assets, t.Commission, t.MarginRate, t.Strategy, t.RollOver }).ForeignKey("dbo.Codes", t => t.Code).Index(t => t.Code);
            AddPrimaryKey("dbo.ImitationGames", new[] { "BaseShort", "BaseLong", "NonaTime", "NonaShort", "NonaLong", "OctaTime", "OctaShort", "OctaLong", "HeptaTime", "HeptaShort", "HeptaLong", "HexaTime", "HexaShort", "HexaLong", "PentaTime", "PentaShort", "PentaLong", "QuadTime", "QuadShort", "QuadLong", "TriTime", "TriShort", "TriLong", "DuoTime", "DuoShort", "DuoLong", "MonoTime", "MonoShort", "MonoLong", "Date" });
            DropColumn("dbo.ImitationGames", "Strategy");
            DropColumn("dbo.ImitationGames", "RollOver");
            DropColumn("dbo.ImitationGames", "MarginRate");
            DropColumn("dbo.ImitationGames", "Code");
            DropColumn("dbo.ImitationGames", "Assets");
            DropColumn("dbo.ImitationGames", "Commission");
        }
        public override void Down()
        {
            AddColumn("dbo.ImitationGames", "Commission", c => c.Double(nullable: false));
            AddColumn("dbo.ImitationGames", "Assets", c => c.Long(nullable: false));
            AddColumn("dbo.ImitationGames", "Code", c => c.String(maxLength: 8));
            AddColumn("dbo.ImitationGames", "MarginRate", c => c.Double(nullable: false));
            AddColumn("dbo.ImitationGames", "RollOver", c => c.Boolean(nullable: false));
            AddColumn("dbo.ImitationGames", "Strategy", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.Statistics", "Code", "dbo.Codes");
            DropIndex("dbo.Statistics", new[] { "Code" });
            DropPrimaryKey("dbo.ImitationGames");
            DropTable("dbo.Statistics");
            AddPrimaryKey("dbo.ImitationGames", new[] { "Strategy", "RollOver", "BaseShort", "BaseLong", "NonaTime", "NonaShort", "NonaLong", "OctaTime", "OctaShort", "OctaLong", "HeptaTime", "HeptaShort", "HeptaLong", "HexaTime", "HexaShort", "HexaLong", "PentaTime", "PentaShort", "PentaLong", "QuadTime", "QuadShort", "QuadLong", "TriTime", "TriShort", "TriLong", "DuoTime", "DuoShort", "DuoLong", "MonoTime", "MonoShort", "MonoLong", "Date" });
            CreateIndex("dbo.ImitationGames", "Code");
            AddForeignKey("dbo.ImitationGames", "Code", "dbo.Codes", "Code");
        }
    }
}