using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class imagez : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<byte[]>(
                name: "UserImage",
                schema: "prms",
                table: "SalesPerformances",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserImage",
                schema: "prms",
                table: "SalesPerformances");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "prms",
                table: "SalesPerformances",
                type: "nvarchar(450)",
                nullable: true);

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
                principalColumn: "Id");
        }
    }
}
