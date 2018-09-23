using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Fixes_Second_Round : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EFProjectImplementors_Organizations_ImplementorId",
                table: "EFProjectImplementors");

            migrationBuilder.DropForeignKey(
                name: "FK_EFProjectImplementors_Projects_ProjectId",
                table: "EFProjectImplementors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EFProjectImplementors",
                table: "EFProjectImplementors");

            migrationBuilder.RenameTable(
                name: "EFProjectImplementors",
                newName: "ProjectImplementors");

            migrationBuilder.RenameIndex(
                name: "IX_EFProjectImplementors_ImplementorId",
                table: "ProjectImplementors",
                newName: "IX_ProjectImplementors_ImplementorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectImplementors",
                table: "ProjectImplementors",
                columns: new[] { "ProjectId", "ImplementorId" });

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
                name: "FK_ProjectImplementors_Organizations_ImplementorId",
                table: "ProjectImplementors",
                column: "ImplementorId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectImplementors_Projects_ProjectId",
                table: "ProjectImplementors",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectImplementors_Organizations_ImplementorId",
                table: "ProjectImplementors");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectImplementors_Projects_ProjectId",
                table: "ProjectImplementors");

            migrationBuilder.DropTable(
                name: "ProjectFundings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectImplementors",
                table: "ProjectImplementors");

            migrationBuilder.RenameTable(
                name: "ProjectImplementors",
                newName: "EFProjectImplementors");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectImplementors_ImplementorId",
                table: "EFProjectImplementors",
                newName: "IX_EFProjectImplementors_ImplementorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EFProjectImplementors",
                table: "EFProjectImplementors",
                columns: new[] { "ProjectId", "ImplementorId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EFProjectImplementors_Organizations_ImplementorId",
                table: "EFProjectImplementors",
                column: "ImplementorId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EFProjectImplementors_Projects_ProjectId",
                table: "EFProjectImplementors",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
