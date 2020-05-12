using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Key_Project_Membership : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembershipRequests",
                table: "ProjectMembershipRequests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembershipRequests",
                table: "ProjectMembershipRequests",
                columns: new[] { "ProjectId", "UserId", "MembershipType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembershipRequests",
                table: "ProjectMembershipRequests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembershipRequests",
                table: "ProjectMembershipRequests",
                columns: new[] { "ProjectId", "UserId" });
        }
    }
}
