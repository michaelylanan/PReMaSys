using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PReMaSys.Migrations
{
    public partial class ImprovedBusinessReqInputs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BIRTIN",
                schema: "prms",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessPNumber",
                schema: "prms",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BusinessType",
                schema: "prms",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyEmail",
                schema: "prms",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyWebsite",
                schema: "prms",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfEmployees",
                schema: "prms",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SECNumber",
                schema: "prms",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BIRTIN",
                schema: "prms",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BusinessPNumber",
                schema: "prms",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BusinessType",
                schema: "prms",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompanyEmail",
                schema: "prms",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompanyWebsite",
                schema: "prms",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NumberOfEmployees",
                schema: "prms",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SECNumber",
                schema: "prms",
                table: "Users");
        }
    }
}
