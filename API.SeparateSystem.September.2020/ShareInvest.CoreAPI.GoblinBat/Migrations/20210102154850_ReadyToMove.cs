using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class ReadyToMove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Securities",
                columns: table => new
                {
                    Identify = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Strategics = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Contents = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Security = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Securities", x => new { x.Identify, x.Code });
                    table.ForeignKey(
                        name: "FK_Securities_Codes_Code",
                        column: x => x.Code,
                        principalTable: "Codes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Securities_Privacies_Security",
                        column: x => x.Security,
                        principalTable: "Privacies",
                        principalColumn: "Security",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ticks",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Date = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Open = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Close = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contents = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticks", x => new { x.Code, x.Date });
                    table.ForeignKey(
                        name: "FK_Ticks_Codes_Code",
                        column: x => x.Code,
                        principalTable: "Codes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Securities_Code",
                table: "Securities",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Securities_Security",
                table: "Securities",
                column: "Security");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Securities");

            migrationBuilder.DropTable(
                name: "Ticks");
        }
    }
}
