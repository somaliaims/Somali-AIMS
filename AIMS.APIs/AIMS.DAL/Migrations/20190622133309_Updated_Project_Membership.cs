using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Project_Membership : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembershipRequests",
                table: "ProjectMembershipRequests");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "ProjectMembershipRequests");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ProjectMembershipRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembershipRequests",
                table: "ProjectMembershipRequests",
                columns: new[] { "ProjectId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembershipRequests_UserId",
                table: "ProjectMembershipRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembershipRequests_Projects_ProjectId",
                table: "ProjectMembershipRequests",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembershipRequests_Users_UserId",
                table: "ProjectMembershipRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembershipRequests_Projects_ProjectId",
                table: "ProjectMembershipRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembershipRequests_Users_UserId",
                table: "ProjectMembershipRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembershipRequests",
                table: "ProjectMembershipRequests");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMembershipRequests_UserId",
                table: "ProjectMembershipRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProjectMembershipRequests");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "ProjectMembershipRequests",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembershipRequests",
                table: "ProjectMembershipRequests",
                columns: new[] { "ProjectId", "UserEmail" });
        }
    }
}
