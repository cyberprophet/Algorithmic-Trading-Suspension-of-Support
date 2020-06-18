using System.Data.Entity.Migrations;

namespace ShareInvest.Main.Migrations
{
    public partial class MonoMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable("dbo.FileGoblinBats", c => new
            {
                Name = c.String(nullable: false, maxLength: 128),
                Version = c.String(nullable: false, maxLength: 7),
                Buffer = c.Binary(nullable: false),
            }).PrimaryKey(t => new { t.Name, t.Version });
        }
        public override void Down() => DropTable("dbo.FileGoblinBats");
    }
}