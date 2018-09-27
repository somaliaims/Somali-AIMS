using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Sectors_For_Mapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Sectors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Sectors",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EFSectorTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypeName = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFSectorTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EFSectorCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<string>(nullable: true),
                    SectorTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFSectorCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EFSectorCategory_EFSectorTypes_SectorTypeId",
                        column: x => x.SectorTypeId,
                        principalTable: "EFSectorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EFSectorSubCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SectorCategoryId = table.Column<int>(nullable: false),
                    SubCategory = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFSectorSubCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EFSectorSubCategory_EFSectorCategory_SectorCategoryId",
                        column: x => x.SectorCategoryId,
                        principalTable: "EFSectorCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_CategoryId",
                table: "Sectors",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_SubCategoryId",
                table: "Sectors",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EFSectorCategory_SectorTypeId",
                table: "EFSectorCategory",
                column: "SectorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EFSectorSubCategory_SectorCategoryId",
                table: "EFSectorSubCategory",
                column: "SectorCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sectors_EFSectorCategory_CategoryId",
                table: "Sectors",
                column: "CategoryId",
                principalTable: "EFSectorCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sectors_EFSectorSubCategory_SubCategoryId",
                table: "Sectors",
                column: "SubCategoryId",
                principalTable: "EFSectorSubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sectors_EFSectorCategory_CategoryId",
                table: "Sectors");

            migrationBuilder.DropForeignKey(
                name: "FK_Sectors_EFSectorSubCategory_SubCategoryId",
                table: "Sectors");

            migrationBuilder.DropTable(
                name: "EFSectorSubCategory");

            migrationBuilder.DropTable(
                name: "EFSectorCategory");

            migrationBuilder.DropTable(
                name: "EFSectorTypes");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_CategoryId",
                table: "Sectors");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_SubCategoryId",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Sectors");
        }
    }
}
