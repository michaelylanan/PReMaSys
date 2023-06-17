using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class _1234 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesPerformances_Users_UserId",
                schema: "prms",
                table: "SalesPerformances");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "prms",
                table: "SalesPerformances",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesPerformances_Users_UserId",
                schema: "prms",
                table: "SalesPerformances",
                column: "UserId",
                principalSchema: "prms",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesPerformances_Users_UserId",
                schema: "prms",
                table: "SalesPerformances");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "prms",
                table: "SalesPerformances",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesPerformances_Users_UserId",
                schema: "prms",
                table: "SalesPerformances",
                column: "UserId",
                principalSchema: "prms",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
