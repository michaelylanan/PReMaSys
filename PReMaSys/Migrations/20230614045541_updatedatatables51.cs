using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class updatedatatables51 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DailyForecast",
                schema: "prms",
                table: "SalesForecasts",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyForecast",
                schema: "prms",
                table: "SalesForecasts");
        }
    }
}
