using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Columns_Envelope : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SectorAmounts",
                table: "Envelope",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SectorExpectedAmounts",
                table: "Envelope",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SectorManualAmounts",
                table: "Envelope",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectorAmounts",
                table: "Envelope");

            migrationBuilder.DropColumn(
                name: "SectorExpectedAmounts",
                table: "Envelope");

            migrationBuilder.DropColumn(
                name: "SectorManualAmounts",
                table: "Envelope");
        }
    }
}
