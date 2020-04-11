using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class DoDeca : DbMigration
    {
        public override void Up()
        {
            CreateTable("dbo.Logs", c => new
            {
                Identity = c.String(nullable: false, maxLength: 20),
                Date = c.String(nullable: false, maxLength: 6),
                Assets = c.String(nullable: false, maxLength: 4),
                Strategy = c.String(nullable: false, maxLength: 4),
                Commission = c.String(nullable: false, maxLength: 2),
                RollOver = c.String(nullable: false, maxLength: 1),
                Code = c.String(nullable: false, maxLength: 6),
            }).PrimaryKey(t => new { t.Identity, t.Date });
        }
        public override void Down()
        {
            DropTable("dbo.Logs");
        }
    }
}