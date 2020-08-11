using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareInvest.Migrations
{
    public partial class DuoCoreAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(name: "Coin", table: "Privacies", type: "float", nullable: false, defaultValue: 0.0);
            migrationBuilder.CreateTable(name: "Catalog", columns: table => new
            {
                Strategics = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Short = table.Column<int>(type: "int", nullable: false),
                Long = table.Column<int>(type: "int", nullable: false),
                Trend = table.Column<int>(type: "int", nullable: false),
                RealizeProfit = table.Column<double>(type: "float", nullable: false),
                AdditionalPurchase = table.Column<double>(type: "float", nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                QuoteUnit = table.Column<int>(type: "int", nullable: false),
                LongShort = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                TrendType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                Setting = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true)
            }, constraints: table => table.PrimaryKey("PK_Catalog", x => x.Strategics));
            migrationBuilder.CreateTable(name: "StocksStrategics", columns: table => new
            {
                Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                Strategics = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Date = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                MaximumInvestment = table.Column<long>(type: "bigint", nullable: false),
                CumulativeReturn = table.Column<double>(type: "float", nullable: false),
                WeightedAverageDailyReturn = table.Column<double>(type: "float", nullable: false),
                DiscrepancyRateFromExpectedStockPrice = table.Column<double>(type: "float", nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_StocksStrategics", x => new { x.Code, x.Strategics });
                table.ForeignKey(name: "FK_StocksStrategics_Catalog_Strategics", column: x => x.Strategics, principalTable: "Catalog", principalColumn: "Strategics", onDelete: ReferentialAction.Cascade);
                table.ForeignKey(name: "FK_StocksStrategics_Codes_Code", column: x => x.Code, principalTable: "Codes", principalColumn: "Code", onDelete: ReferentialAction.Cascade);
            });
            migrationBuilder.CreateIndex(name: "IX_StocksStrategics_Strategics", table: "StocksStrategics", column: "Strategics");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "StocksStrategics");
            migrationBuilder.DropTable(name: "Catalog");
            migrationBuilder.DropColumn(name: "Coin", table: "Privacies");
        }
    }
}