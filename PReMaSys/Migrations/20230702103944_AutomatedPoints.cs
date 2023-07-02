using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class AutomatedPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerformanceCriterias",
                schema: "prms",
                columns: table => new
                {
                    PerformanceCriteriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RewardsCriteria = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceCriterias", x => x.PerformanceCriteriaId);
                });

            migrationBuilder.CreateTable(
                name: "PointsAllocation",
                schema: "prms",
                columns: table => new
                {
                    PointsAllocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PerformanceCriteriaId = table.Column<int>(type: "int", nullable: false),
                    TimeLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformancePoints = table.Column<int>(type: "int", nullable: false),
                    CriteriaQuota = table.Column<int>(type: "int", nullable: true),
                    DateAded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsAllocation", x => x.PointsAllocationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerformanceCriterias",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "PointsAllocation",
                schema: "prms");
        }
    }
}
