using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Notificatoins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifications_Organizations_OrganizationId",
                table: "UserNotifications");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "UserNotifications",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifications_Organizations_OrganizationId",
                table: "UserNotifications",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifications_Organizations_OrganizationId",
                table: "UserNotifications");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "UserNotifications",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifications_Organizations_OrganizationId",
                table: "UserNotifications",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
