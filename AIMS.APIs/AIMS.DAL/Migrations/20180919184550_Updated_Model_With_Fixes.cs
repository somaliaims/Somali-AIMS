using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Model_With_Fixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "DateStarted",
                table: "Projects",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "DateEnded",
                table: "Projects",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "OrganizationTypes",
                newName: "TypeName");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "Sectors",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Locations",
                type: "decimal(9, 2)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Locations",
                type: "decimal(9, 2)",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "Sectors");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Projects",
                newName: "DateStarted");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Projects",
                newName: "DateEnded");

            migrationBuilder.RenameColumn(
                name: "TypeName",
                table: "OrganizationTypes",
                newName: "Type");

            migrationBuilder.AlterColumn<int>(
                name: "Longitude",
                table: "Locations",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9, 2)");

            migrationBuilder.AlterColumn<int>(
                name: "Latitude",
                table: "Locations",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9, 2)");
        }
    }
}
