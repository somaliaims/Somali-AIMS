using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Unaffiliated_OrgTypes_RemovedFrom_SectorTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUnAffiliated",
                table: "SectorTypes");

            migrationBuilder.AddColumn<bool>(
                name: "IsUnAffiliated",
                table: "OrganizationTypes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUnAffiliated",
                table: "OrganizationTypes");

            migrationBuilder.AddColumn<bool>(
                name: "IsUnAffiliated",
                table: "SectorTypes",
                nullable: false,
                defaultValue: false);
        }
    }
}
