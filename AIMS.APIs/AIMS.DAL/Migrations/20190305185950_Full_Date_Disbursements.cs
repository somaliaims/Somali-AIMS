using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Full_Date_Disbursements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "ProjectDisbursements");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "ProjectDisbursements");

            migrationBuilder.AddColumn<DateTime>(
                name: "Dated",
                table: "ProjectDisbursements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements",
                columns: new[] { "ProjectId", "Dated" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements");

            migrationBuilder.DropColumn(
                name: "Dated",
                table: "ProjectDisbursements");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "ProjectDisbursements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "ProjectDisbursements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectDisbursements",
                table: "ProjectDisbursements",
                columns: new[] { "ProjectId", "Year", "Month" });
        }
    }
}
