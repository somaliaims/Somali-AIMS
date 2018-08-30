using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Fixes_First_Round : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationUsers");

            migrationBuilder.DropTable(
                name: "SectorCodes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "ProjectFunders");

            migrationBuilder.DropColumn(
                name: "AmountInUSD",
                table: "ProjectFunders");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "ProjectFunders");

            migrationBuilder.DropColumn(
                name: "FinancialYear",
                table: "ProjectFunders");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Organizations");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Organizations",
                newName: "Name");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Sectors",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "EFProjectImplementers",
                columns: table => new
                {
                    ImplementerId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFProjectImplementers", x => new { x.ProjectId, x.ImplementerId });
                    table.ForeignKey(
                        name: "FK_EFProjectImplementers_Organizations_ImplementerId",
                        column: x => x.ImplementerId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EFProjectImplementers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_OrganizationId",
                table: "Users",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EFProjectImplementers_ImplementerId",
                table: "EFProjectImplementers",
                column: "ImplementerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Organizations_OrganizationId",
                table: "Users",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Organizations_OrganizationId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "EFProjectImplementers");

            migrationBuilder.DropIndex(
                name: "IX_Users_OrganizationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Sectors");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Organizations",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Sectors",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "ProjectFunders",
                type: "decimal(9 ,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountInUSD",
                table: "ProjectFunders",
                type: "decimal(9, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "ProjectFunders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinancialYear",
                table: "ProjectFunders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Organizations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Organizations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrganizationUsers",
                columns: table => new
                {
                    OrganizationId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUsers", x => new { x.OrganizationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_OrganizationUsers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectorCodes",
                columns: table => new
                {
                    SectorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MappingCode = table.Column<string>(nullable: true),
                    SectorId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectorCodes", x => x.SectorId);
                    table.ForeignKey(
                        name: "FK_SectorCodes_Sectors_SectorId1",
                        column: x => x.SectorId1,
                        principalTable: "Sectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_UserId",
                table: "OrganizationUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SectorCodes_SectorId1",
                table: "SectorCodes",
                column: "SectorId1");
        }
    }
}
