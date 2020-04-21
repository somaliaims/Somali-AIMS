using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Envelope_Merge_Organizations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnvelopeOrganizationId",
                table: "OrganizationsToMerge",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationsToMerge_EnvelopeOrganizationId",
                table: "OrganizationsToMerge",
                column: "EnvelopeOrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationsToMerge_Organizations_EnvelopeOrganizationId",
                table: "OrganizationsToMerge",
                column: "EnvelopeOrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationsToMerge_Organizations_EnvelopeOrganizationId",
                table: "OrganizationsToMerge");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationsToMerge_EnvelopeOrganizationId",
                table: "OrganizationsToMerge");

            migrationBuilder.DropColumn(
                name: "EnvelopeOrganizationId",
                table: "OrganizationsToMerge");
        }
    }
}
