using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Project_Deletion_Requests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDeletionRequests_Users_UserId",
                table: "ProjectDeletionRequests");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ProjectDeletionRequests",
                newName: "RequestedById");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectDeletionRequests_UserId",
                table: "ProjectDeletionRequests",
                newName: "IX_ProjectDeletionRequests_RequestedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDeletionRequests_Users_RequestedById",
                table: "ProjectDeletionRequests",
                column: "RequestedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDeletionRequests_Users_RequestedById",
                table: "ProjectDeletionRequests");

            migrationBuilder.RenameColumn(
                name: "RequestedById",
                table: "ProjectDeletionRequests",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectDeletionRequests_RequestedById",
                table: "ProjectDeletionRequests",
                newName: "IX_ProjectDeletionRequests_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDeletionRequests_Users_UserId",
                table: "ProjectDeletionRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
