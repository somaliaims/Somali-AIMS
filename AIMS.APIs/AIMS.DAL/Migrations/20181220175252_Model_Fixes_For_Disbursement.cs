using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Model_Fixes_For_Disbursement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_ProjectDisbursements_Id",
                table: "ProjectDisbursements");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProjectDisbursements");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProjectDisbursements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ProjectDisbursements_Id",
                table: "ProjectDisbursements",
                column: "Id");
        }
    }
}
