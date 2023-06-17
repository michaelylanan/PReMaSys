using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class UpdateSalesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesPerformances",
                schema: "prms",
                columns: table => new
                {
                    SalesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalesRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TargetSales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalesVolume = table.Column<int>(type: "int", nullable: false),
                    ConversionR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AverageDealSize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerAcquisition = table.Column<int>(type: "int", nullable: false),
                    CustomerRetentionR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ForcastSalesRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesPerformances", x => x.SalesID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesPerformances",
                schema: "prms");
        }
    }
}
