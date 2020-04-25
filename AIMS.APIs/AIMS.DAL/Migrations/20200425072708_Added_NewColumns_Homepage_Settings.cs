using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_NewColumns_Homepage_Settings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AIMSBarTitle",
                table: "HomePageSettings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaviconPath",
                table: "HomePageSettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIMSBarTitle",
                table: "HomePageSettings");

            migrationBuilder.DropColumn(
                name: "FaviconPath",
                table: "HomePageSettings");
        }
    }
}
