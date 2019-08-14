using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Organization_Types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationTypeId",
                table: "Organizations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OrganizationTypeId",
                table: "Organizations",
                column: "OrganizationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_OrganizationTypes_OrganizationTypeId",
                table: "Organizations",
                column: "OrganizationTypeId",
                principalTable: "OrganizationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_OrganizationTypes_OrganizationTypeId",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_OrganizationTypeId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "OrganizationTypeId",
                table: "Organizations");
        }
    }
}
