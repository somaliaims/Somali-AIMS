using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Funder_Implementer_Requests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EFProjectImplementerRequests",
                columns: table => new
                {
                    ImplementerId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    RequestDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFProjectImplementerRequests", x => new { x.ProjectId, x.ImplementerId });
                    table.ForeignKey(
                        name: "FK_EFProjectImplementerRequests_Organizations_ImplementerId",
                        column: x => x.ImplementerId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EFProjectImplementerRequests_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FunderRequests",
                columns: table => new
                {
                    FunderId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    FundingTypeId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(11 ,2)", nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(9, 2)", nullable: false),
                    Dated = table.Column<DateTime>(nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_EFProjectImplementerRequests_ImplementerId",
                table: "EFProjectImplementerRequests",
                column: "ImplementerId");

            migrationBuilder.CreateIndex(
                name: "IX_FunderRequests_FunderId",
                table: "FunderRequests",
                column: "FunderId");

            migrationBuilder.CreateIndex(
                name: "IX_FunderRequests_FundingTypeId",
                table: "FunderRequests",
                column: "FundingTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EFProjectImplementerRequests");

            migrationBuilder.DropTable(
                name: "FunderRequests");
        }
    }
}
