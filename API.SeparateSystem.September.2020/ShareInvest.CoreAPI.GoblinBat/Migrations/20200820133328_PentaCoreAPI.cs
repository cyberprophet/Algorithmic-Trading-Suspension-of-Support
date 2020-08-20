using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class PentaCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "File", columns: table => new
            {
                Version = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
            }, constraints: table => table.PrimaryKey("PK_File", x => x.Version));
        }
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "File");
    }
}