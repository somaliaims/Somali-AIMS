using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_RequestedBy_OrgMergs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestedById",
                table: "OrganizationMergeRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMergeRequests_RequestedById",
                table: "OrganizationMergeRequests",
                column: "RequestedById");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMergeRequests_Users_RequestedById",
                table: "OrganizationMergeRequests",
                column: "RequestedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMergeRequests_Users_RequestedById",
                table: "OrganizationMergeRequests");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMergeRequests_RequestedById",
                table: "OrganizationMergeRequests");

            migrationBuilder.DropColumn(
                name: "RequestedById",
                table: "OrganizationMergeRequests");
        }
    }
}
