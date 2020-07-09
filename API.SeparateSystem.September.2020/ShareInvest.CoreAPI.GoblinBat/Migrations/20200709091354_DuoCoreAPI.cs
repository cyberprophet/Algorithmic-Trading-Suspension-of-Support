using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class DuoCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AlterColumn<string>(name: "SecuritiesAPI", table: "Privacies", type: "nvarchar(1)", maxLength: 1, nullable: true, oldClrType: typeof(string), oldType: "nvarchar(1)", oldMaxLength: 1);
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.AlterColumn<string>(name: "SecuritiesAPI", table: "Privacies", type: "nvarchar(1)", maxLength: 1, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(1)", oldMaxLength: 1, oldNullable: true);
    }
}