using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class NonaCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "Incorporate", columns: table => new
            {
                Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Date = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                Capitalization = table.Column<int>(type: "int", nullable: false),
                Market = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Incorporate", x => x.Code);
                table.ForeignKey(name: "FK_Incorporate_Codes_Code", column: x => x.Code, principalTable: "Codes", principalColumn: "Code", onDelete: ReferentialAction.Cascade);
            });
        }
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Incorporate");
    }
}