using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Fixed_Names_ProjectOrg_Types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_EFOrganizationTypes_OrganizationTypeId",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_EFProjectTypes_ProjectTypeId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EFProjectTypes",
                table: "EFProjectTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EFOrganizationTypes",
                table: "EFOrganizationTypes");

            migrationBuilder.RenameTable(
                name: "EFProjectTypes",
                newName: "ProjectTypes");

            migrationBuilder.RenameTable(
                name: "EFOrganizationTypes",
                newName: "OrganizationTypes");

            migrationBuilder.RenameColumn(
                name: "ProjectType",
                table: "ProjectTypes",
                newName: "Type");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExchangeRate",
                table: "ProjectSectors",
                type: "decimal(9, 2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTypes",
                table: "ProjectTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationTypes",
                table: "OrganizationTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_OrganizationTypes_OrganizationTypeId",
                table: "Organizations",
                column: "OrganizationTypeId",
                principalTable: "OrganizationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectTypes_ProjectTypeId",
                table: "Projects",
                column: "ProjectTypeId",
                principalTable: "ProjectTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_OrganizationTypes_OrganizationTypeId",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectTypes_ProjectTypeId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTypes",
                table: "ProjectTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationTypes",
                table: "OrganizationTypes");

            migrationBuilder.RenameTable(
                name: "ProjectTypes",
                newName: "EFProjectTypes");

            migrationBuilder.RenameTable(
                name: "OrganizationTypes",
                newName: "EFOrganizationTypes");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "EFProjectTypes",
                newName: "ProjectType");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExchangeRate",
                table: "ProjectSectors",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9, 2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EFProjectTypes",
                table: "EFProjectTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EFOrganizationTypes",
                table: "EFOrganizationTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_EFOrganizationTypes_OrganizationTypeId",
                table: "Organizations",
                column: "OrganizationTypeId",
                principalTable: "EFOrganizationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_EFProjectTypes_ProjectTypeId",
                table: "Projects",
                column: "ProjectTypeId",
                principalTable: "EFProjectTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
