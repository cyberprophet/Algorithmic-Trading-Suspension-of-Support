using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class QuarterCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Financials_Codes_Code",
                table: "Financials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Financials",
                table: "Financials");

            migrationBuilder.RenameTable(
                name: "Financials",
                newName: "FinancialStatement");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FinancialStatement",
                table: "FinancialStatement",
                columns: new[] { "Code", "Date" });

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialStatement_Codes_Code",
                table: "FinancialStatement",
                column: "Code",
                principalTable: "Codes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialStatement_Codes_Code",
                table: "FinancialStatement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FinancialStatement",
                table: "FinancialStatement");

            migrationBuilder.RenameTable(
                name: "FinancialStatement",
                newName: "Financials");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Financials",
                table: "Financials",
                columns: new[] { "Code", "Date" });

            migrationBuilder.AddForeignKey(
                name: "FK_Financials_Codes_Code",
                table: "Financials",
                column: "Code",
                principalTable: "Codes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
