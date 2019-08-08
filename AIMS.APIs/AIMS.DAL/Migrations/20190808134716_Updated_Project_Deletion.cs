using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Project_Deletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDeletionRequests_Users_RequestedById",
                table: "ProjectDeletionRequests");

            migrationBuilder.DropIndex(
                name: "IX_ProjectDeletionRequests_RequestedById",
                table: "ProjectDeletionRequests");

            migrationBuilder.DropColumn(
                name: "RequestedById",
                table: "ProjectDeletionRequests");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeletionRequests_UserId",
                table: "ProjectDeletionRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDeletionRequests_Users_UserId",
                table: "ProjectDeletionRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDeletionRequests_Users_UserId",
                table: "ProjectDeletionRequests");

            migrationBuilder.DropIndex(
                name: "IX_ProjectDeletionRequests_UserId",
                table: "ProjectDeletionRequests");

            migrationBuilder.AddColumn<int>(
                name: "RequestedById",
                table: "ProjectDeletionRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeletionRequests_RequestedById",
                table: "ProjectDeletionRequests",
                column: "RequestedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDeletionRequests_Users_RequestedById",
                table: "ProjectDeletionRequests",
                column: "RequestedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
