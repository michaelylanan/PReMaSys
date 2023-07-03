using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class MakeSERIDString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SERecord_Users_SERIdId",
                schema: "prms",
                table: "SERecord");

            migrationBuilder.DropIndex(
                name: "IX_SERecord_SERIdId",
                schema: "prms",
                table: "SERecord");

            migrationBuilder.DropColumn(
                name: "SERIdId",
                schema: "prms",
                table: "SERecord");

            migrationBuilder.AddColumn<string>(
                name: "SERId",
                schema: "prms",
                table: "SERecord",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SERId",
                schema: "prms",
                table: "SERecord");

            migrationBuilder.AddColumn<string>(
                name: "SERIdId",
                schema: "prms",
                table: "SERecord",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SERecord_SERIdId",
                schema: "prms",
                table: "SERecord",
                column: "SERIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_SERecord_Users_SERIdId",
                schema: "prms",
                table: "SERecord",
                column: "SERIdId",
                principalSchema: "prms",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
