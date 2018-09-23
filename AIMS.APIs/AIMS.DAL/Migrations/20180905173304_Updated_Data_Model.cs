using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Data_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCustomFields_Projects_ProjectId",
                table: "ProjectCustomFields");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCustomFields_Sectors_SectorId",
                table: "ProjectCustomFields");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Locations_LocationId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Sectors_SectorId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectLogs");

            migrationBuilder.DropTable(
                name: "UserSubscriptions");

            migrationBuilder.DropTable(
                name: "TimeIntervals");

            migrationBuilder.DropIndex(
                name: "IX_Projects_LocationId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_SectorId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectFundings",
                table: "ProjectFundings");

            migrationBuilder.DropIndex(
                name: "IX_ProjectFundings_ProjectId",
                table: "ProjectFundings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectCustomFields",
                table: "ProjectCustomFields");

            migrationBuilder.DropIndex(
                name: "IX_ProjectCustomFields_ProjectId",
                table: "ProjectCustomFields");

            migrationBuilder.DropIndex(
                name: "IX_ProjectCustomFields_SectorId",
                table: "ProjectCustomFields");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "SectorId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProjectFundings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProjectCustomFields");

            migrationBuilder.DropColumn(
                name: "SectorId",
                table: "ProjectCustomFields");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Sectors",
                newName: "SectorName");

            migrationBuilder.RenameColumn(
                name: "FieldType",
                table: "ProjectCustomFields",
                newName: "CustomFieldId");

            migrationBuilder.RenameColumn(
                name: "FieldTitle",
                table: "ProjectCustomFields",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Organizations",
                newName: "OrganizationName");

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                table: "UserNotifications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeId",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "ProjectImplementors",
                type: "decimal(9 ,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "ProjectImplementors",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "ProjectImplementors",
                type: "decimal(9, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "FunderId",
                table: "ProjectFundings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "ProjectCustomFields",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationTypeId",
                table: "Organizations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EFProjectId",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectFundings",
                table: "ProjectFundings",
                columns: new[] { "ProjectId", "FunderId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectCustomFields",
                table: "ProjectCustomFields",
                columns: new[] { "ProjectId", "CustomFieldId" });

            migrationBuilder.CreateTable(
                name: "CustomFields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FieldTitle = table.Column<string>(nullable: true),
                    FieldType = table.Column<int>(nullable: false),
                    ActiveFrom = table.Column<DateTime>(nullable: false),
                    ActiveUpto = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EFOrganizationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFOrganizationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EFProjectTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFProjectTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    OldValues = table.Column<string>(nullable: true),
                    NewValues = table.Column<string>(nullable: true),
                    UpdatedById = table.Column<int>(nullable: true),
                    ActionPerformed = table.Column<int>(nullable: false),
                    Dated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Logs_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectDisbursements",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    StartingYear = table.Column<int>(nullable: false),
                    StartingMonth = table.Column<int>(nullable: false),
                    EndingYear = table.Column<int>(nullable: false),
                    EndingMonth = table.Column<int>(nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(9, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDisbursements", x => new { x.ProjectId, x.StartingYear });
                    table.ForeignKey(
                        name: "FK_ProjectDisbursements_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    DocumentTitle = table.Column<string>(nullable: true),
                    DocumentUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectDocuments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLocations",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(9, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLocations", x => new { x.ProjectId, x.LocationId });
                    table.ForeignKey(
                        name: "FK_ProjectLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectLocations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMarkers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    Marker = table.Column<string>(nullable: true),
                    Percentage = table.Column<decimal>(type: "decimal(9, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMarkers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMarkers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSectors",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    SectorId = table.Column<int>(nullable: false),
                    ContributedAmount = table.Column<decimal>(type: "decimal(9, 2)", nullable: false),
                    ExchangeRate = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSectors", x => new { x.ProjectId, x.SectorId });
                    table.ForeignKey(
                        name: "FK_ProjectSectors_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSectors_Sectors_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportSubscriptions",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    ReportId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSubscriptions", x => new { x.UserId, x.ReportId });
                    table.ForeignKey(
                        name: "FK_ReportSubscriptions_StaticReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "StaticReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportSubscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectTypeId",
                table: "Projects",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundings_FunderId",
                table: "ProjectFundings",
                column: "FunderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCustomFields_CustomFieldId",
                table: "ProjectCustomFields",
                column: "CustomFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OrganizationTypeId",
                table: "Organizations",
                column: "OrganizationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_EFProjectId",
                table: "Locations",
                column: "EFProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ProjectId",
                table: "Logs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UpdatedById",
                table: "Logs",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDocuments_ProjectId",
                table: "ProjectDocuments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLocations_LocationId",
                table: "ProjectLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMarkers_ProjectId",
                table: "ProjectMarkers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSectors_SectorId",
                table: "ProjectSectors",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSubscriptions_ReportId",
                table: "ReportSubscriptions",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Projects_EFProjectId",
                table: "Locations",
                column: "EFProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_EFOrganizationTypes_OrganizationTypeId",
                table: "Organizations",
                column: "OrganizationTypeId",
                principalTable: "EFOrganizationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCustomFields_CustomFields_CustomFieldId",
                table: "ProjectCustomFields",
                column: "CustomFieldId",
                principalTable: "CustomFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCustomFields_Projects_ProjectId",
                table: "ProjectCustomFields",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectFundings_Organizations_FunderId",
                table: "ProjectFundings",
                column: "FunderId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_EFProjectTypes_ProjectTypeId",
                table: "Projects",
                column: "ProjectTypeId",
                principalTable: "EFProjectTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Projects_EFProjectId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_EFOrganizationTypes_OrganizationTypeId",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCustomFields_CustomFields_CustomFieldId",
                table: "ProjectCustomFields");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCustomFields_Projects_ProjectId",
                table: "ProjectCustomFields");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectFundings_Organizations_FunderId",
                table: "ProjectFundings");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_EFProjectTypes_ProjectTypeId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "CustomFields");

            migrationBuilder.DropTable(
                name: "EFOrganizationTypes");

            migrationBuilder.DropTable(
                name: "EFProjectTypes");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "ProjectDisbursements");

            migrationBuilder.DropTable(
                name: "ProjectDocuments");

            migrationBuilder.DropTable(
                name: "ProjectLocations");

            migrationBuilder.DropTable(
                name: "ProjectMarkers");

            migrationBuilder.DropTable(
                name: "ProjectSectors");

            migrationBuilder.DropTable(
                name: "ReportSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectTypeId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectFundings",
                table: "ProjectFundings");

            migrationBuilder.DropIndex(
                name: "IX_ProjectFundings_FunderId",
                table: "ProjectFundings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectCustomFields",
                table: "ProjectCustomFields");

            migrationBuilder.DropIndex(
                name: "IX_ProjectCustomFields_CustomFieldId",
                table: "ProjectCustomFields");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_OrganizationTypeId",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_EFProjectId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "ProjectTypeId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "ProjectImplementors");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "ProjectImplementors");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "ProjectImplementors");

            migrationBuilder.DropColumn(
                name: "FunderId",
                table: "ProjectFundings");

            migrationBuilder.DropColumn(
                name: "OrganizationTypeId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "EFProjectId",
                table: "Locations");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SectorName",
                table: "Sectors",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "ProjectCustomFields",
                newName: "FieldTitle");

            migrationBuilder.RenameColumn(
                name: "CustomFieldId",
                table: "ProjectCustomFields",
                newName: "FieldType");

            migrationBuilder.RenameColumn(
                name: "OrganizationName",
                table: "Organizations",
                newName: "Name");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Sectors",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectorId",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProjectFundings",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "ProjectCustomFields",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProjectCustomFields",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "SectorId",
                table: "ProjectCustomFields",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectFundings",
                table: "ProjectFundings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectCustomFields",
                table: "ProjectCustomFields",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProjectLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Dated = table.Column<DateTime>(nullable: false),
                    NewValues = table.Column<string>(nullable: true),
                    OldValues = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectLogs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectLogs_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeIntervals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DurationInMonths = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeIntervals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSubscriptions",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    ReportId = table.Column<int>(nullable: false),
                    DateSubscribed = table.Column<DateTime>(nullable: false),
                    IntervalId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    ReportSentOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptions", x => new { x.UserId, x.ReportId });
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_TimeIntervals_IntervalId",
                        column: x => x.IntervalId,
                        principalTable: "TimeIntervals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_StaticReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "StaticReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LocationId",
                table: "Projects",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_SectorId",
                table: "Projects",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundings_ProjectId",
                table: "ProjectFundings",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCustomFields_ProjectId",
                table: "ProjectCustomFields",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCustomFields_SectorId",
                table: "ProjectCustomFields",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLogs_ProjectId",
                table: "ProjectLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLogs_UpdatedById",
                table: "ProjectLogs",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_IntervalId",
                table: "UserSubscriptions",
                column: "IntervalId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_ReportId",
                table: "UserSubscriptions",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCustomFields_Projects_ProjectId",
                table: "ProjectCustomFields",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCustomFields_Sectors_SectorId",
                table: "ProjectCustomFields",
                column: "SectorId",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Locations_LocationId",
                table: "Projects",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Sectors_SectorId",
                table: "Projects",
                column: "SectorId",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
