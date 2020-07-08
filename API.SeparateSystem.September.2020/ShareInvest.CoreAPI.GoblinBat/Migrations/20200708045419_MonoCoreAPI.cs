using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class MonoCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.CreateTable(name: "Privacies", columns: table => new { Security = table.Column<string>(type: "nvarchar(450)", nullable: false) }, constraints: table => table.PrimaryKey("PK_Privacies", x => x.Security));
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Privacies");
    }
}