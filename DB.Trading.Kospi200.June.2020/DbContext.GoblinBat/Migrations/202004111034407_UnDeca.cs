using System.Data.Entity.Migrations;

namespace ShareInvest.GoblinBatContext.Migrations
{
    public partial class UnDeca : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Strategics", "Code", c => c.String(nullable: false, maxLength: 6));
            AlterColumn("dbo.Strategics", "Strategy", c => c.String(nullable: false, maxLength: 4));
        }
        public override void Down()
        {
            AlterColumn("dbo.Strategics", "Strategy", c => c.String(nullable: false));
            AlterColumn("dbo.Strategics", "Code", c => c.String(nullable: false, maxLength: 8));
        }
    }
}