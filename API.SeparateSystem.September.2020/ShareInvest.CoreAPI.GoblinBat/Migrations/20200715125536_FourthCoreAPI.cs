using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class FourthCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodeStrategics",
                table: "Privacies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Commission",
                table: "Privacies",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MarginRate",
                table: "Codes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeStrategics",
                table: "Privacies");

            migrationBuilder.DropColumn(
                name: "Commission",
                table: "Privacies");

            migrationBuilder.DropColumn(
                name: "MarginRate",
                table: "Codes");
        }
    }
}
