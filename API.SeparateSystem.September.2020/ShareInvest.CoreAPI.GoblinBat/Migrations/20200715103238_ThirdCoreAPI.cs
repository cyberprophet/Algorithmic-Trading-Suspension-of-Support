using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class ThirdCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_Code",
                table: "Stocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Options",
                table: "Options");

            migrationBuilder.DropIndex(
                name: "IX_Options_Code",
                table: "Options");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Futures",
                table: "Futures");

            migrationBuilder.DropIndex(
                name: "IX_Futures_Code",
                table: "Futures");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks",
                columns: new[] { "Code", "Date" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Options",
                table: "Options",
                columns: new[] { "Code", "Date" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Futures",
                table: "Futures",
                columns: new[] { "Code", "Date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Options",
                table: "Options");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Futures",
                table: "Futures");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks",
                column: "Date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Options",
                table: "Options",
                column: "Date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Futures",
                table: "Futures",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_Code",
                table: "Stocks",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Options_Code",
                table: "Options",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Futures_Code",
                table: "Futures",
                column: "Code");
        }
    }
}
