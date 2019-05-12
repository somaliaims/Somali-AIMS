﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Initial_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Currency = table.Column<string>(nullable: true),
                    CurrencyName = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    IsNational = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomFields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FieldTitle = table.Column<string>(nullable: true),
                    FieldType = table.Column<int>(nullable: false),
                    Values = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MessageType = table.Column<int>(nullable: false),
                    TypeDefinition = table.Column<string>(maxLength: 100, nullable: true),
                    Message = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExchangeRatesJson = table.Column<string>(nullable: true),
                    Dated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRatesAPIsCount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Dated = table.Column<DateTime>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRatesAPIsCount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRatesSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsAutomatic = table.Column<bool>(nullable: false),
                    APIKeyOpenExchangeRates = table.Column<string>(nullable: true),
                    ManualExchangeRates = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRatesSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinancialYears",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FinancialYear = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FundingTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FundingType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundingTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IATIData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Data = table.Column<string>(nullable: true),
                    Organizations = table.Column<string>(nullable: true),
                    Dated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IATIData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IATISettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BaseUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IATISettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Location = table.Column<string>(nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(9, 5)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9, 5)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManualExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Dated = table.Column<DateTime>(nullable: false),
                    ExchangeRate = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualExchangeRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrganizationName = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: false),
                    RegisteredOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
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
                name: "PasswordRecoveryRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    Dated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordRecoveryRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 1000, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SectorMappings",
                columns: table => new
                {
                    SectorId = table.Column<int>(nullable: false),
                    MappedSectorId = table.Column<int>(nullable: false),
                    SectorTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectorMappings", x => new { x.SectorId, x.MappedSectorId });
                });

            migrationBuilder.CreateTable(
                name: "SectorTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypeName = table.Column<string>(nullable: true),
                    IsPrimary = table.Column<bool>(nullable: true),
                    IsIATIType = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectorTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SMTPSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Host = table.Column<string>(nullable: true),
                    Port = table.Column<int>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    AdminEmail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMTPSettings", x => x.Id);
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
                name: "Envelope",
                columns: table => new
                {
                    FunderId = table.Column<int>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(9, 2)", nullable: false),
                    SectorAmountsBreakup = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Envelope", x => x.FunderId);
                    table.ForeignKey(
                        name: "FK_Envelope_Organizations_FunderId",
                        column: x => x.FunderId,
                        principalTable: "Organizations",
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
                    TreatmentId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Dated = table.Column<DateTime>(nullable: false),
                    IsSeen = table.Column<bool>(nullable: false),
                    NotificationType = table.Column<int>(nullable: false)
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
                    UserType = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: false),
                    ApprovedById = table.Column<int>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: false),
                    ApprovedOn = table.Column<DateTime>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    LastLogin = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Users_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectCustomFields",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    CustomFieldId = table.Column<int>(nullable: false),
                    FieldType = table.Column<int>(nullable: false),
                    Values = table.Column<string>(nullable: true)
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
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    Dated = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(9, 2)", nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(9, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDisbursements", x => x.Id);
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
                    FundingTypeId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(9 ,2)", nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(9, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFunders", x => new { x.ProjectId, x.FunderId, x.FundingTypeId });
                    table.ForeignKey(
                        name: "FK_ProjectFunders_Organizations_FunderId",
                        column: x => x.FunderId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectFunders_FundingTypes_FundingTypeId",
                        column: x => x.FundingTypeId,
                        principalTable: "FundingTypes",
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
                name: "ProjectImplementers",
                columns: table => new
                {
                    ImplementerId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectImplementers", x => new { x.ProjectId, x.ImplementerId });
                    table.ForeignKey(
                        name: "FK_ProjectImplementers_Organizations_ImplementerId",
                        column: x => x.ImplementerId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectImplementers_Projects_ProjectId",
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
                    FundsPercentage = table.Column<decimal>(type: "decimal(9, 2)", nullable: false)
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
                name: "Sectors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SectorTypeId = table.Column<int>(nullable: false),
                    SectorName = table.Column<string>(nullable: true),
                    ParentSectorId = table.Column<int>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sectors_Sectors_ParentSectorId",
                        column: x => x.ParentSectorId,
                        principalTable: "Sectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sectors_SectorTypes_SectorTypeId",
                        column: x => x.SectorTypeId,
                        principalTable: "SectorTypes",
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
                name: "ProjectSectors",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    SectorId = table.Column<int>(nullable: false),
                    FundsPercentage = table.Column<decimal>(type: "decimal(9, 2)", nullable: false)
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
                name: "IX_Currencies_Currency",
                table: "Currencies",
                column: "Currency",
                unique: true,
                filter: "[Currency] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialYears_FinancialYear",
                table: "FinancialYears",
                column: "FinancialYear",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ProjectId",
                table: "Logs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UpdatedById",
                table: "Logs",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCustomFields_CustomFieldId",
                table: "ProjectCustomFields",
                column: "CustomFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDisbursements_ProjectId",
                table: "ProjectDisbursements",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDocuments_ProjectId",
                table: "ProjectDocuments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFunders_FunderId",
                table: "ProjectFunders",
                column: "FunderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFunders_FundingTypeId",
                table: "ProjectFunders",
                column: "FundingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectImplementers_ImplementerId",
                table: "ProjectImplementers",
                column: "ImplementerId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_ParentSectorId",
                table: "Sectors",
                column: "ParentSectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_SectorTypeId",
                table: "Sectors",
                column: "SectorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_OrganizationId",
                table: "UserNotifications",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ApprovedById",
                table: "Users",
                column: "ApprovedById");

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
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "EmailMessages");

            migrationBuilder.DropTable(
                name: "Envelope");

            migrationBuilder.DropTable(
                name: "ExchangeRates");

            migrationBuilder.DropTable(
                name: "ExchangeRatesAPIsCount");

            migrationBuilder.DropTable(
                name: "ExchangeRatesSettings");

            migrationBuilder.DropTable(
                name: "FinancialYears");

            migrationBuilder.DropTable(
                name: "IATIData");

            migrationBuilder.DropTable(
                name: "IATISettings");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "ManualExchangeRates");

            migrationBuilder.DropTable(
                name: "OrganizationTypes");

            migrationBuilder.DropTable(
                name: "PasswordRecoveryRequests");

            migrationBuilder.DropTable(
                name: "ProjectCustomFields");

            migrationBuilder.DropTable(
                name: "ProjectDisbursements");

            migrationBuilder.DropTable(
                name: "ProjectDocuments");

            migrationBuilder.DropTable(
                name: "ProjectFunders");

            migrationBuilder.DropTable(
                name: "ProjectImplementers");

            migrationBuilder.DropTable(
                name: "ProjectLocations");

            migrationBuilder.DropTable(
                name: "ProjectMarkers");

            migrationBuilder.DropTable(
                name: "ProjectSectors");

            migrationBuilder.DropTable(
                name: "ReportSubscriptions");

            migrationBuilder.DropTable(
                name: "SectorMappings");

            migrationBuilder.DropTable(
                name: "SMTPSettings");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "CustomFields");

            migrationBuilder.DropTable(
                name: "FundingTypes");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Sectors");

            migrationBuilder.DropTable(
                name: "StaticReports");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SectorTypes");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}