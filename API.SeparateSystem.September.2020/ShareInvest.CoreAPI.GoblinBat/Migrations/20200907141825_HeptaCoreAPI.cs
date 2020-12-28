using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class HeptaCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "Financials", columns: table => new
            {
                Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Date = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Revenues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IncomeFromOperations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IncomeFromOperation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ProfitFromContinuingOperations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NetIncome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ControllingNetIncome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NonControllingNetIncome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TotalAssets = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TotalLiabilites = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TotalEquity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ControllingEquity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NonControllingEquity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                EquityCapital = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OperatingActivities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvestingActivities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FinancingActivities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CAPEX = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FCF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InterestAccruingLiabilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OperatingMargin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NetMargin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ROE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ROA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebtRatio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RetentionRatio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                EPS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                BPS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PBR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DPS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DividendYield = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PayoutRatio = table.Column<string>(type: "nvarchar(max)", nullable: true)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Financials", x => new { x.Code, x.Date });
                table.ForeignKey(name: "FK_Financials_Codes_Code", column: x => x.Code, principalTable: "Codes", principalColumn: "Code", onDelete: ReferentialAction.Cascade);
            });
        }
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Financials");
    }
}