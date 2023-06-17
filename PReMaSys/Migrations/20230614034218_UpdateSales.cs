using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class UpdateSales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForcastSalesRevenue",
                schema: "prms",
                table: "SalesPerformances");

            migrationBuilder.CreateTable(
                name: "SalesForecasts",
                schema: "prms",
                columns: table => new
                {
                    ForecastID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SPID = table.Column<int>(type: "int", nullable: false),
                    WeeklyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuarterlyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YearlyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesForecasts", x => x.ForecastID);
                    table.ForeignKey(
                        name: "FK_SalesForecasts_SalesPerformances_SPID",
                        column: x => x.SPID,
                        principalSchema: "prms",
                        principalTable: "SalesPerformances",
                        principalColumn: "SalesID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesForecasts_SPID",
                schema: "prms",
                table: "SalesForecasts",
                column: "SPID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesForecasts",
                schema: "prms");

            migrationBuilder.AddColumn<decimal>(
                name: "ForcastSalesRevenue",
                schema: "prms",
                table: "SalesPerformances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
