using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class NewlyaddedTableTracker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatePointGiven",
                schema: "prms",
                table: "SERecord");

            migrationBuilder.CreateTable(
                name: "PointsTracker",
                schema: "prms",
                columns: table => new
                {
                    PointsTrackerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PerformanceCriteriaId = table.Column<int>(type: "int", nullable: false),
                    SalesPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformancePoints = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsTracker", x => x.PointsTrackerId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointsTracker",
                schema: "prms");

            migrationBuilder.AddColumn<DateTime>(
                name: "DatePointGiven",
                schema: "prms",
                table: "SERecord",
                type: "datetime2",
                nullable: true);
        }
    }
}
