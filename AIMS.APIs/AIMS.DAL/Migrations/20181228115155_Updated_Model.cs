using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EFProjectId",
                table: "Sectors",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_EFProjectId",
                table: "Sectors",
                column: "EFProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sectors_Projects_EFProjectId",
                table: "Sectors",
                column: "EFProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sectors_Projects_EFProjectId",
                table: "Sectors");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_EFProjectId",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "EFProjectId",
                table: "Sectors");
        }
    }
}
