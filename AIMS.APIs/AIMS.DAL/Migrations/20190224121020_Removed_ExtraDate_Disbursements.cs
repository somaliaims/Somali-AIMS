using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Removed_ExtraDate_Disbursements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements");

            migrationBuilder.DropColumn(
                name: "StartingYear",
                table: "ProjectDisbursements");

            migrationBuilder.DropColumn(
                name: "StartingMonth",
                table: "ProjectDisbursements");

            migrationBuilder.RenameColumn(
                name: "EndingYear",
                table: "ProjectDisbursements",
                newName: "Month");

            migrationBuilder.RenameColumn(
                name: "EndingMonth",
                table: "ProjectDisbursements",
                newName: "Year");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements",
                columns: new[] { "ProjectId", "Year", "Month" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements");

            migrationBuilder.RenameColumn(
                name: "Month",
                table: "ProjectDisbursements",
                newName: "EndingYear");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "ProjectDisbursements",
                newName: "EndingMonth");

            migrationBuilder.AddColumn<int>(
                name: "StartingYear",
                table: "ProjectDisbursements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartingMonth",
                table: "ProjectDisbursements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements",
                columns: new[] { "ProjectId", "StartingYear", "StartingMonth" });
        }
    }
}
