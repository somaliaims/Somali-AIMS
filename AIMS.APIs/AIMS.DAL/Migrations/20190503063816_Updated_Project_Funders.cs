using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Project_Funders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GrantTypeId",
                table: "ProjectFunders",
                nullable: false
               );

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFunders_GrantTypeId",
                table: "ProjectFunders",
                column: "GrantTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectFunders_GrantTypes_GrantTypeId",
                table: "ProjectFunders",
                column: "GrantTypeId",
                principalTable: "GrantTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectFunders_GrantTypes_GrantTypeId",
                table: "ProjectFunders");

            migrationBuilder.DropIndex(
                name: "IX_ProjectFunders_GrantTypeId",
                table: "ProjectFunders");

            migrationBuilder.DropColumn(
                name: "GrantTypeId",
                table: "ProjectFunders");
        }
    }
}
