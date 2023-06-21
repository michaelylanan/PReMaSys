using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class RevisedTablez : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "EmployeeofThes",
                schema: "prms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalesPerformanceSalesID = table.Column<int>(type: "int", nullable: false),
                    EmployeeOfTheMonth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeOfTheYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeofThes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeofThes_SalesPerformances_SalesPerformanceSalesID",
                        column: x => x.SalesPerformanceSalesID,
                        principalSchema: "prms",
                        principalTable: "SalesPerformances",
                        principalColumn: "SalesID",
                        onDelete: ReferentialAction.Cascade);
                });

           

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "EmployeeofThes",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "SalesForecast",
                schema: "prms");

            migrationBuilder.DropTable(
                name: "SalesPerformances",
                schema: "prms");

            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "prms",
                table: "Rewards");
        }
    }
}
