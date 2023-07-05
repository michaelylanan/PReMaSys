using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class AddAUSerecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupportIdId",
                schema: "prms",
                table: "SERecord",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SERecord_SupportIdId",
                schema: "prms",
                table: "SERecord",
                column: "SupportIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_SERecord_Users_SupportIdId",
                schema: "prms",
                table: "SERecord",
                column: "SupportIdId",
                principalSchema: "prms",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SERecord_Users_SupportIdId",
                schema: "prms",
                table: "SERecord");

            migrationBuilder.DropIndex(
                name: "IX_SERecord_SupportIdId",
                schema: "prms",
                table: "SERecord");

            migrationBuilder.DropColumn(
                name: "SupportIdId",
                schema: "prms",
                table: "SERecord");
        }
    }
}
