using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class MonoCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "Codes", columns: table => new
            {
                Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                MaturityMarketCap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                MarginRate = table.Column<double>(type: "float", nullable: false),
                Price = table.Column<string>(type: "nvarchar(max)", nullable: true)
            }, constraints: table => table.PrimaryKey("PK_Codes", x => x.Code));
            migrationBuilder.CreateTable(name: "Privacies", columns: table => new
            {
                Security = table.Column<string>(type: "nvarchar(450)", nullable: false),
                SecuritiesAPI = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                SecurityAPI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Account = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                Commission = table.Column<double>(type: "float", nullable: false),
                CodeStrategics = table.Column<string>(type: "nvarchar(max)", nullable: true)
            }, constraints: table => table.PrimaryKey("PK_Privacies", x => x.Security));
            migrationBuilder.CreateTable(name: "Days", columns: table => new
            {
                Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Date = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Price = table.Column<string>(type: "nvarchar(max)", nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Days", x => new { x.Code, x.Date });
                table.ForeignKey(
                    name: "FK_Days_Codes_Code",
                    column: x => x.Code,
                    principalTable: "Codes",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Cascade);
            });
            migrationBuilder.CreateTable(name: "Futures", columns: table => new
            {
                Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Date = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Volume = table.Column<int>(type: "int", nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Futures", x => new { x.Code, x.Date });
                table.ForeignKey(
                    name: "FK_Futures_Codes_Code",
                    column: x => x.Code,
                    principalTable: "Codes",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Cascade);
            });
            migrationBuilder.CreateTable(name: "Options", columns: table => new
            {
                Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Date = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Volume = table.Column<int>(type: "int", nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Options", x => new { x.Code, x.Date });
                table.ForeignKey(
                    name: "FK_Options_Codes_Code",
                    column: x => x.Code,
                    principalTable: "Codes",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Cascade);
            });
            migrationBuilder.CreateTable(name: "Stocks", columns: table => new
            {
                Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Date = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Volume = table.Column<int>(type: "int", nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Stocks", x => new { x.Code, x.Date });
                table.ForeignKey(
                    name: "FK_Stocks_Codes_Code",
                    column: x => x.Code,
                    principalTable: "Codes",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Cascade);
            });
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Days");
            migrationBuilder.DropTable(name: "Futures");
            migrationBuilder.DropTable(name: "Options");
            migrationBuilder.DropTable(name: "Privacies");
            migrationBuilder.DropTable(name: "Stocks");
            migrationBuilder.DropTable(name: "Codes");
        }
    }
}