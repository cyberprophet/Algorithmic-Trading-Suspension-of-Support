using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class TriCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "RevisedStockPrices", columns: table => new
            {
                Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Date = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                Rate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Revise = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_RevisedStockPrices", x => new { x.Code, x.Date });
                table.ForeignKey(name: "FK_RevisedStockPrices_Codes_Code", column: x => x.Code, principalTable: "Codes", principalColumn: "Code", onDelete: ReferentialAction.Cascade);
            });
        }
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "RevisedStockPrices");
    }
}