using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Removed_IsViewed_From_ContactMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRegistered",
                table: "ContactMessages");

            migrationBuilder.AddColumn<int>(
                name: "ContactType",
                table: "ContactMessages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "ContactMessages",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "ContactMessages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "ContactMessages",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "ContactMessages",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_ProjectId",
                table: "ContactMessages",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_Projects_ProjectId",
                table: "ContactMessages",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_Projects_ProjectId",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_ProjectId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "ContactType",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "ContactMessages");

            migrationBuilder.AddColumn<bool>(
                name: "IsRegistered",
                table: "ContactMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
