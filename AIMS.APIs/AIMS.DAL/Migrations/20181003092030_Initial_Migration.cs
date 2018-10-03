using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Initial_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "OrganizationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypeName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SectorTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypeName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectorTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaticReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrganizationName = table.Column<string>(nullable: true),
                    OrganizationTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_OrganizationTypes_OrganizationTypeId",
                        column: x => x.OrganizationTypeId,
                        principalTable: "OrganizationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 100, nullable: true),
                    Objective = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    ProjectTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_ProjectTypes_ProjectTypeId",
                        column: x => x.ProjectTypeId,
                        principalTable: "ProjectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SectorCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<string>(nullable: true),
                    SectorTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectorCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectorCategories_SectorTypes_SectorTypeId",
                        column: x => x.SectorTypeId,
                        principalTable: "SectorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserType = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Dated = table.Column<DateTime>(nullable: false),
                    IsSeen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotifications_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    UserType = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Location = table.Column<string>(nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(9, 2)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9, 2)", nullable: false),
                    EFProjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Projects_EFProjectId",
                        column: x => x.EFProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectCustomFields",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    CustomFieldId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectCustomFields", x => new { x.ProjectId, x.CustomFieldId });
                    table.ForeignKey(
                        name: "FK_ProjectCustomFields_CustomFields_CustomFieldId",
                        column: x => x.CustomFieldId,
                        principalTable: "CustomFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectCustomFields_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectDisbursements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
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
                name: "ProjectFunders",
                columns: table => new
                {
                    FunderId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(9 ,2)", nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(9, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFunders", x => new { x.ProjectId, x.FunderId });
                    table.ForeignKey(
                        name: "FK_ProjectFunders_Organizations_FunderId",
                        column: x => x.FunderId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectFunders_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFundings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FunderId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    GrantType = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(9 ,2)", nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(9, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFundings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectFundings_Organizations_FunderId",
                        column: x => x.FunderId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectFundings_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectImplementors",
                columns: table => new
                {
                    ImplementorId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectImplementors", x => new { x.ProjectId, x.ImplementorId });
                    table.ForeignKey(
                        name: "FK_ProjectImplementors_Organizations_ImplementorId",
                        column: x => x.ImplementorId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectImplementors_Projects_ProjectId",
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
                name: "SectorSubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SectorCategoryId = table.Column<int>(nullable: false),
                    SubCategory = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectorSubCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectorSubCategories_SectorCategories_SectorCategoryId",
                        column: x => x.SectorCategoryId,
                        principalTable: "SectorCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Sectors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SectorName = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    SubCategoryId = table.Column<int>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sectors_SectorCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "SectorCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sectors_SectorSubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SectorSubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSectors",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    SectorId = table.Column<int>(nullable: false),
                    ContributedAmount = table.Column<decimal>(type: "decimal(9, 2)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(9, 2)", nullable: false)
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
                name: "IX_Organizations_OrganizationTypeId",
                table: "Organizations",
                column: "OrganizationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCustomFields_CustomFieldId",
                table: "ProjectCustomFields",
                column: "CustomFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDocuments_ProjectId",
                table: "ProjectDocuments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFunders_FunderId",
                table: "ProjectFunders",
                column: "FunderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundings_FunderId",
                table: "ProjectFundings",
                column: "FunderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundings_ProjectId",
                table: "ProjectFundings",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectImplementors_ImplementorId",
                table: "ProjectImplementors",
                column: "ImplementorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLocations_LocationId",
                table: "ProjectLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMarkers_ProjectId",
                table: "ProjectMarkers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectTypeId",
                table: "Projects",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSectors_SectorId",
                table: "ProjectSectors",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSubscriptions_ReportId",
                table: "ReportSubscriptions",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_SectorCategories_SectorTypeId",
                table: "SectorCategories",
                column: "SectorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_CategoryId",
                table: "Sectors",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_SubCategoryId",
                table: "Sectors",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SectorSubCategories_SectorCategoryId",
                table: "SectorSubCategories",
                column: "SectorCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_OrganizationId",
                table: "UserNotifications",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OrganizationId",
                table: "Users",
                column: "OrganizationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "ProjectCustomFields");

            migrationBuilder.DropTable(
                name: "ProjectDisbursements");

            migrationBuilder.DropTable(
                name: "ProjectDocuments");

            migrationBuilder.DropTable(
                name: "ProjectFunders");

            migrationBuilder.DropTable(
                name: "ProjectFundings");

            migrationBuilder.DropTable(
                name: "ProjectImplementors");

            migrationBuilder.DropTable(
                name: "ProjectLocations");

            migrationBuilder.DropTable(
                name: "ProjectMarkers");

            migrationBuilder.DropTable(
                name: "ProjectSectors");

            migrationBuilder.DropTable(
                name: "ReportSubscriptions");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "CustomFields");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Sectors");

            migrationBuilder.DropTable(
                name: "StaticReports");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "SectorSubCategories");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "ProjectTypes");

            migrationBuilder.DropTable(
                name: "SectorCategories");

            migrationBuilder.DropTable(
                name: "OrganizationTypes");

            migrationBuilder.DropTable(
                name: "SectorTypes");
        }
    }
}
