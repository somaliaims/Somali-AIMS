using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Fixes_Second_Round : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EFProjectImplementers_Organizations_ImplementerId",
                table: "EFProjectImplementers");

            migrationBuilder.DropForeignKey(
                name: "FK_EFProjectImplementers_Projects_ProjectId",
                table: "EFProjectImplementers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EFProjectImplementers",
                table: "EFProjectImplementers");

            migrationBuilder.RenameTable(
                name: "EFProjectImplementers",
                newName: "ProjectImplementers");

            migrationBuilder.RenameIndex(
                name: "IX_EFProjectImplementers_ImplementerId",
                table: "ProjectImplementers",
                newName: "IX_ProjectImplementers_ImplementerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectImplementers",
                table: "ProjectImplementers",
                columns: new[] { "ProjectId", "ImplementerId" });

            migrationBuilder.CreateTable(
                name: "ProjectFundings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(9 ,2)", nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(9, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFundings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectFundings_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundings_ProjectId",
                table: "ProjectFundings",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectImplementers_Organizations_ImplementerId",
                table: "ProjectImplementers",
                column: "ImplementerId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectImplementers_Projects_ProjectId",
                table: "ProjectImplementers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectImplementers_Organizations_ImplementerId",
                table: "ProjectImplementers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectImplementers_Projects_ProjectId",
                table: "ProjectImplementers");

            migrationBuilder.DropTable(
                name: "ProjectFundings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectImplementers",
                table: "ProjectImplementers");

            migrationBuilder.RenameTable(
                name: "ProjectImplementers",
                newName: "EFProjectImplementers");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectImplementers_ImplementerId",
                table: "EFProjectImplementers",
                newName: "IX_EFProjectImplementers_ImplementerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EFProjectImplementers",
                table: "EFProjectImplementers",
                columns: new[] { "ProjectId", "ImplementerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EFProjectImplementers_Organizations_ImplementerId",
                table: "EFProjectImplementers",
                column: "ImplementerId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EFProjectImplementers_Projects_ProjectId",
                table: "EFProjectImplementers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
