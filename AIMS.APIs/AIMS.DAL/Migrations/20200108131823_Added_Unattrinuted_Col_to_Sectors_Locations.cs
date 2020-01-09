using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Unattrinuted_Col_to_Sectors_Locations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUnAttributed",
                table: "Sectors",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnAttributed",
                table: "Locations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUnAttributed",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "IsUnAttributed",
                table: "Locations");
        }
    }
}
