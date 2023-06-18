using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class NewTablez : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "inventory",
                schema: "prms",
                table: "Rewards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "prms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesPerformances",
                schema: "prms",
                columns: table => new
                {
                    SalesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitsSold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostPricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SellingPricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Particulars = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalesRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalesProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalesVolume = table.Column<int>(type: "int", nullable: false),
                    ConversionR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AverageDealSize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerAcquisition = table.Column<int>(type: "int", nullable: false),
                    CustomerRetentionR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesPerformances", x => x.SalesID);
                });

            migrationBuilder.CreateTable(
                name: "SalesForecasts",
                schema: "prms",
                columns: table => new
                {
                    ForecastID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SPID = table.Column<int>(type: "int", nullable: false),
                    SalesPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DailyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WeeklyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonthlyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    QuarterlyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    YearlyForecast = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
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
                name: "AuditLogs",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "SalesForecasts",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "SalesPerformances",
                schema: "prms");

            migrationBuilder.DropColumn(
                name: "inventory",
                schema: "prms",
                table: "Rewards");
        }
    }
}
