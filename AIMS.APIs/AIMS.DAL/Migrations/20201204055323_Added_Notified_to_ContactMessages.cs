using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Notified_to_ContactMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsViewed",
                table: "ContactMessages");

            migrationBuilder.AddColumn<bool>(
                name: "IsNotified",
                table: "ContactMessages",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNotified",
                table: "ContactMessages");

            migrationBuilder.AddColumn<bool>(
                name: "IsViewed",
                table: "ContactMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
