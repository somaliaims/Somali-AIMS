using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_ColName_Envelope : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectorAmounts",
                table: "Envelope");

            migrationBuilder.DropColumn(
                name: "SectorExpectedAmounts",
                table: "Envelope");

            migrationBuilder.RenameColumn(
                name: "SectorManualAmounts",
                table: "Envelope",
                newName: "SectorAmountsBreakup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SectorAmountsBreakup",
                table: "Envelope",
                newName: "SectorManualAmounts");

            migrationBuilder.AddColumn<string>(
                name: "SectorAmounts",
                table: "Envelope",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SectorExpectedAmounts",
                table: "Envelope",
                nullable: true);
        }
    }
}
