using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_PK_Disbursements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements",
                columns: new[] { "ProjectId", "StartingYear", "StartingMonth" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements",
                columns: new[] { "ProjectId", "StartingYear" });
        }
    }
}
