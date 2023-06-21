using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class UpdateEmployeeOfThe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeofThes_SalesPerformances_SalesPerformanceSalesID",
                schema: "prms",
                table: "EmployeeofThes");

            migrationBuilder.AlterColumn<int>(
                name: "SalesPerformanceSalesID",
                schema: "prms",
                table: "EmployeeofThes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeofThes_SalesPerformances_SalesPerformanceSalesID",
                schema: "prms",
                table: "EmployeeofThes",
                column: "SalesPerformanceSalesID",
                principalSchema: "prms",
                principalTable: "SalesPerformances",
                principalColumn: "SalesID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeofThes_SalesPerformances_SalesPerformanceSalesID",
                schema: "prms",
                table: "EmployeeofThes");

            migrationBuilder.AlterColumn<int>(
                name: "SalesPerformanceSalesID",
                schema: "prms",
                table: "EmployeeofThes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeofThes_SalesPerformances_SalesPerformanceSalesID",
                schema: "prms",
                table: "EmployeeofThes",
                column: "SalesPerformanceSalesID",
                principalSchema: "prms",
                principalTable: "SalesPerformances",
                principalColumn: "SalesID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
