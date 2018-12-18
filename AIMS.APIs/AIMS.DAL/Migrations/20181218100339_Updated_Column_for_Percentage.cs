using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Column_for_Percentage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllocatedAmount",
                table: "ProjectSectors",
                newName: "FundsPercentage");

            migrationBuilder.RenameColumn(
                name: "Percentage",
                table: "ProjectLocations",
                newName: "FundsPercentage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FundsPercentage",
                table: "ProjectSectors",
                newName: "AllocatedAmount");

            migrationBuilder.RenameColumn(
                name: "FundsPercentage",
                table: "ProjectLocations",
                newName: "Percentage");
        }
    }
}
