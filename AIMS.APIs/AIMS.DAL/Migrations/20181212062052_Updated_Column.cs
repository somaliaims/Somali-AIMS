using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContributedAmount",
                table: "ProjectSectors",
                newName: "AllocatedAmount");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "ProjectSectors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "ProjectSectors");

            migrationBuilder.RenameColumn(
                name: "AllocatedAmount",
                table: "ProjectSectors",
                newName: "ContributedAmount");
        }
    }
}
