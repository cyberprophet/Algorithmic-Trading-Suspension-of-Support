using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class QuadCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "Consensus", columns: table => new
            {
                Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Date = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Quarter = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                Sales = table.Column<long>(type: "bigint", nullable: false),
                YoY = table.Column<double>(type: "float", nullable: false),
                Op = table.Column<long>(type: "bigint", nullable: false),
                Np = table.Column<long>(type: "bigint", nullable: false),
                Eps = table.Column<int>(type: "int", nullable: false),
                Bps = table.Column<int>(type: "int", nullable: false),
                Per = table.Column<double>(type: "float", nullable: false),
                Pbr = table.Column<double>(type: "float", nullable: false),
                Roe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Ev = table.Column<string>(type: "nvarchar(max)", nullable: true)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Consensus", x => new { x.Code, x.Date, x.Quarter });
                table.ForeignKey(name: "FK_Consensus_Codes_Code", column: x => x.Code, principalTable: "Codes", principalColumn: "Code", onDelete: ReferentialAction.Cascade);
            });
        }
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Consensus");
    }
}