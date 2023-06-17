using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class UpdateSalesTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetSales",
                schema: "prms",
                table: "SalesPerformances",
                newName: "UnitsSold");

            migrationBuilder.AlterColumn<decimal>(
                name: "SalesRevenue",
                schema: "prms",
                table: "SalesPerformances",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "CostPricePerUnit",
                schema: "prms",
                table: "SalesPerformances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Particulars",
                schema: "prms",
                table: "SalesPerformances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SellingPricePerUnit",
                schema: "prms",
                table: "SalesPerformances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UnitType",
                schema: "prms",
                table: "SalesPerformances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPricePerUnit",
                schema: "prms",
                table: "SalesPerformances");

            migrationBuilder.DropColumn(
                name: "Particulars",
                schema: "prms",
                table: "SalesPerformances");

            migrationBuilder.DropColumn(
                name: "SellingPricePerUnit",
                schema: "prms",
                table: "SalesPerformances");

            migrationBuilder.DropColumn(
                name: "UnitType",
                schema: "prms",
                table: "SalesPerformances");

            migrationBuilder.RenameColumn(
                name: "UnitsSold",
                schema: "prms",
                table: "SalesPerformances",
                newName: "TargetSales");

            migrationBuilder.AlterColumn<decimal>(
                name: "SalesRevenue",
                schema: "prms",
                table: "SalesPerformances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
