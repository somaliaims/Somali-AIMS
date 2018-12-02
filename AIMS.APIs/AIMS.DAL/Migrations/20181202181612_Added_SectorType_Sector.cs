using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_SectorType_Sector : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectorTypeId",
                table: "Sectors",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_SectorTypeId",
                table: "Sectors",
                column: "SectorTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sectors_SectorTypes_SectorTypeId",
                table: "Sectors",
                column: "SectorTypeId",
                principalTable: "SectorTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sectors_SectorTypes_SectorTypeId",
                table: "Sectors");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_SectorTypeId",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "SectorTypeId",
                table: "Sectors");
        }
    }
}
