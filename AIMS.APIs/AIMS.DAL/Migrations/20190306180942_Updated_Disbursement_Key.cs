using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Disbursement_Key : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProjectDisbursements",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDisbursements_ProjectId",
                table: "ProjectDisbursements",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements");

            migrationBuilder.DropIndex(
                name: "IX_ProjectDisbursements_ProjectId",
                table: "ProjectDisbursements");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProjectDisbursements");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements",
                columns: new[] { "ProjectId", "Dated" });
        }
    }
}
