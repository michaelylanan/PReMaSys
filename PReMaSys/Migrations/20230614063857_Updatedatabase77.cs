using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class Updatedatabase77 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "prms",
                table: "SalesPerformances",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SalesPerformances_UserId",
                schema: "prms",
                table: "SalesPerformances",
                column: "UserId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesPerformances_Users_UserId",
                schema: "prms",
                table: "SalesPerformances");

            migrationBuilder.DropIndex(
                name: "IX_SalesPerformances_UserId",
                schema: "prms",
                table: "SalesPerformances");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "prms",
                table: "SalesPerformances");
        }
    }
}
