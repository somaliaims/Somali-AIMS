using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Envelope_Merge_Organizations_Reverted_Fixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "EnvelopeOrganizationId",
                table: "OrganizationMergeRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMergeRequests_EnvelopeOrganizationId",
                table: "OrganizationMergeRequests",
                column: "EnvelopeOrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMergeRequests_Organizations_EnvelopeOrganizationId",
                table: "OrganizationMergeRequests",
                column: "EnvelopeOrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMergeRequests_Organizations_EnvelopeOrganizationId",
                table: "OrganizationMergeRequests");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMergeRequests_EnvelopeOrganizationId",
                table: "OrganizationMergeRequests");

            migrationBuilder.DropColumn(
                name: "EnvelopeOrganizationId",
                table: "OrganizationMergeRequests");

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
    }
}
