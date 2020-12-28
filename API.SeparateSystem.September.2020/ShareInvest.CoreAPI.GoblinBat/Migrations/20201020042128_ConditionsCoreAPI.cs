using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class ConditionsCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "Conditions", columns: table => new
            {
                Security = table.Column<string>(type: "nvarchar(450)", nullable: false),
                SettingValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Ban = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Strategics = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TempStorage = table.Column<string>(type: "nvarchar(max)", nullable: true)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Conditions", x => x.Security);
                table.ForeignKey(name: "FK_Conditions_Privacies_Security", column: x => x.Security, principalTable: "Privacies", principalColumn: "Security", onDelete: ReferentialAction.Cascade);
            });
        }
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Conditions");
    }
}