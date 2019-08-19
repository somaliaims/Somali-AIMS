using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Fundtype_Project : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FundingTypeId",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_FundingTypeId",
                table: "Projects",
                column: "FundingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_FundingTypes_FundingTypeId",
                table: "Projects",
                column: "FundingTypeId",
                principalTable: "FundingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_FundingTypes_FundingTypeId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_FundingTypeId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FundingTypeId",
                table: "Projects");
        }
    }
}
