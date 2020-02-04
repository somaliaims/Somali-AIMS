using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Name_Org_Merge_Requests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewName",
                table: "OrganizationMergeRequests",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationIdsJson",
                table: "OrganizationMergeRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewName",
                table: "OrganizationMergeRequests");

            migrationBuilder.DropColumn(
                name: "OrganizationIdsJson",
                table: "OrganizationMergeRequests");
        }
    }
}
