using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_UK_Disbursements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDisbursements_FinancialYears_YearId",
                table: "ProjectDisbursements");

            migrationBuilder.DropIndex(
                name: "IX_ProjectDisbursements_ProjectId",
                table: "ProjectDisbursements");

            migrationBuilder.AlterColumn<int>(
                name: "YearId",
                table: "ProjectDisbursements",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDisbursements_ProjectId_YearId_DisbursementType",
                table: "ProjectDisbursements",
                columns: new[] { "ProjectId", "YearId", "DisbursementType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDisbursements_FinancialYears_YearId",
                table: "ProjectDisbursements",
                column: "YearId",
                principalTable: "FinancialYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDisbursements_FinancialYears_YearId",
                table: "ProjectDisbursements");

            migrationBuilder.DropIndex(
                name: "IX_ProjectDisbursements_ProjectId_YearId_DisbursementType",
                table: "ProjectDisbursements");

            migrationBuilder.AlterColumn<int>(
                name: "YearId",
                table: "ProjectDisbursements",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDisbursements_ProjectId",
                table: "ProjectDisbursements",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDisbursements_FinancialYears_YearId",
                table: "ProjectDisbursements",
                column: "YearId",
                principalTable: "FinancialYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
