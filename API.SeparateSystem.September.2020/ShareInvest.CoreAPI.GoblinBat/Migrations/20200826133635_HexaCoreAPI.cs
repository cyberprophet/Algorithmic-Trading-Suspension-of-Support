using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class HexaCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "Estimate", columns: table => new
            {
                Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Strategics = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Date = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                FirstQuarter = table.Column<double>(type: "float", nullable: false),
                SecondQuarter = table.Column<double>(type: "float", nullable: false),
                ThirdQuarter = table.Column<double>(type: "float", nullable: false),
                Quarter = table.Column<double>(type: "float", nullable: false),
                TheNextYear = table.Column<double>(type: "float", nullable: false),
                TheYearAfterNext = table.Column<double>(type: "float", nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Estimate", x => new { x.Code, x.Strategics });
                table.ForeignKey(name: "FK_Estimate_Catalog_Strategics", column: x => x.Strategics, principalTable: "Catalog", principalColumn: "Strategics", onDelete: ReferentialAction.Cascade);
                table.ForeignKey(name: "FK_Estimate_Codes_Code", column: x => x.Code, principalTable: "Codes", principalColumn: "Code", onDelete: ReferentialAction.Cascade);
            });
            migrationBuilder.CreateIndex(name: "IX_Estimate_Strategics", table: "Estimate", column: "Strategics");
        }
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Estimate");
    }
}