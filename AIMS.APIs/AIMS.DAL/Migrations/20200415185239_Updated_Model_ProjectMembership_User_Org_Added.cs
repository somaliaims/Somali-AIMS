using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Model_ProjectMembership_User_Org_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "ProjectMembershipRequests",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembershipRequests_OrganizationId",
                table: "ProjectMembershipRequests",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembershipRequests_Organizations_OrganizationId",
                table: "ProjectMembershipRequests",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembershipRequests_Organizations_OrganizationId",
                table: "ProjectMembershipRequests");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMembershipRequests_OrganizationId",
                table: "ProjectMembershipRequests");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "ProjectMembershipRequests",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
