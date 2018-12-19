using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Removed_Funds_Extra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectFundings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectFundings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(type: "decimal(9 ,2)", nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(9, 2)", nullable: false),
                    FunderId = table.Column<int>(nullable: false),
                    GrantType = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundings_FunderId",
                table: "ProjectFundings",
                column: "FunderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundings_ProjectId",
                table: "ProjectFundings",
                column: "ProjectId");
        }
    }
}
