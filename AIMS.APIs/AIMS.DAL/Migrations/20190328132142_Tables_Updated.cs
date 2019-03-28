using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Tables_Updated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sectors_EFSectorTypes_SectorTypeId",
                table: "Sectors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EFSectorTypes",
                table: "EFSectorTypes");

            migrationBuilder.RenameTable(
                name: "EFSectorTypes",
                newName: "SectorTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SectorTypes",
                table: "SectorTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sectors_SectorTypes_SectorTypeId",
                table: "Sectors",
                column: "SectorTypeId",
                principalTable: "SectorTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sectors_SectorTypes_SectorTypeId",
                table: "Sectors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SectorTypes",
                table: "SectorTypes");

            migrationBuilder.RenameTable(
                name: "SectorTypes",
                newName: "EFSectorTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EFSectorTypes",
                table: "EFSectorTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sectors_EFSectorTypes_SectorTypeId",
                table: "Sectors",
                column: "SectorTypeId",
                principalTable: "EFSectorTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
