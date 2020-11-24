using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_ColName_ContactMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactType",
                table: "ContactMessages");

            migrationBuilder.AddColumn<int>(
                name: "EmailType",
                table: "ContactMessages",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailType",
                table: "ContactMessages");

            migrationBuilder.AddColumn<int>(
                name: "ContactType",
                table: "ContactMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
