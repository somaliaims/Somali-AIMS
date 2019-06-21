using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Model_ProjectMembershipRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FunderRequests");

            migrationBuilder.DropTable(
                name: "ImplementerRequests");

            migrationBuilder.CreateTable(
                name: "ProjectMembershipRequests",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    UserEmail = table.Column<string>(nullable: false),
                    Dated = table.Column<DateTime>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMembershipRequests", x => new { x.ProjectId, x.UserEmail });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectMembershipRequests");

            migrationBuilder.CreateTable(
                name: "FunderRequests",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    FunderId = table.Column<int>(nullable: false),
                    FundingTypeId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(11 ,2)", nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Dated = table.Column<DateTime>(nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(9, 2)", nullable: false),
                    RequestDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunderRequests", x => new { x.ProjectId, x.FunderId, x.FundingTypeId });
                    table.ForeignKey(
                        name: "FK_FunderRequests_Organizations_FunderId",
                        column: x => x.FunderId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FunderRequests_FundingTypes_FundingTypeId",
                        column: x => x.FundingTypeId,
                        principalTable: "FundingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FunderRequests_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImplementerRequests",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    ImplementerId = table.Column<int>(nullable: false),
                    RequestDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImplementerRequests", x => new { x.ProjectId, x.ImplementerId });
                    table.ForeignKey(
                        name: "FK_ImplementerRequests_Organizations_ImplementerId",
                        column: x => x.ImplementerId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImplementerRequests_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FunderRequests_FunderId",
                table: "FunderRequests",
                column: "FunderId");

            migrationBuilder.CreateIndex(
                name: "IX_FunderRequests_FundingTypeId",
                table: "FunderRequests",
                column: "FundingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ImplementerRequests_ImplementerId",
                table: "ImplementerRequests",
                column: "ImplementerId");
        }
    }
}
