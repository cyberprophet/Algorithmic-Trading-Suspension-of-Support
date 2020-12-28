using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class OctaCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<string>(name: "IssuedStocks", table: "Financials", type: "nvarchar(max)", nullable: true);
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(name: "IssuedStocks", table: "Financials");
    }
}