using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class TriCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<string>(name: "SecurityAPI", table: "Privacies", type: "nvarchar(max)", nullable: true);
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(name: "SecurityAPI", table: "Privacies");
    }
}