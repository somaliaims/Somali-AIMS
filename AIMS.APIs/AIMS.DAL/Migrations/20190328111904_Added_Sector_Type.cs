using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Sector_Type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectorTypeId",
                table: "Sectors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EFSectorTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SectorType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFSectorTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_SectorTypeId",
                table: "Sectors",
                column: "SectorTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sectors_EFSectorTypes_SectorTypeId",
                table: "Sectors",
                column: "SectorTypeId",
                principalTable: "EFSectorTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sectors_EFSectorTypes_SectorTypeId",
                table: "Sectors");

            migrationBuilder.DropTable(
                name: "EFSectorTypes");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_SectorTypeId",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "SectorTypeId",
                table: "Sectors");
        }
    }
}
