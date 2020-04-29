using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_AppliedByAndAutomated_Financial_Transitions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppliedById",
                table: "FinancialTransitions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutomated",
                table: "FinancialTransitions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransitions_AppliedById",
                table: "FinancialTransitions",
                column: "AppliedById");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialTransitions_Users_AppliedById",
                table: "FinancialTransitions",
                column: "AppliedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialTransitions_Users_AppliedById",
                table: "FinancialTransitions");

            migrationBuilder.DropIndex(
                name: "IX_FinancialTransitions_AppliedById",
                table: "FinancialTransitions");

            migrationBuilder.DropColumn(
                name: "AppliedById",
                table: "FinancialTransitions");

            migrationBuilder.DropColumn(
                name: "IsAutomated",
                table: "FinancialTransitions");
        }
    }
}
