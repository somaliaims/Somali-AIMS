using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class ReModel_IATI_Org_Types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IATIOrganizations_OrganizationTypes_OrganizationTypeId",
                table: "IATIOrganizations");

            migrationBuilder.DropIndex(
                name: "IX_IATIOrganizations_OrganizationTypeId",
                table: "IATIOrganizations");

            migrationBuilder.DropColumn(
                name: "OrganizationTypeId",
                table: "IATIOrganizations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationTypeId",
                table: "IATIOrganizations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IATIOrganizations_OrganizationTypeId",
                table: "IATIOrganizations",
                column: "OrganizationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_IATIOrganizations_OrganizationTypes_OrganizationTypeId",
                table: "IATIOrganizations",
                column: "OrganizationTypeId",
                principalTable: "OrganizationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
