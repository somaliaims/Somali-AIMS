using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class FY_Name_Fixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FinacialYear",
                table: "FinancialYears",
                newName: "FinancialYear");

            migrationBuilder.RenameIndex(
                name: "IX_FinancialYears_FinacialYear",
                table: "FinancialYears",
                newName: "IX_FinancialYears_FinancialYear");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FinancialYear",
                table: "FinancialYears",
                newName: "FinacialYear");

            migrationBuilder.RenameIndex(
                name: "IX_FinancialYears_FinancialYear",
                table: "FinancialYears",
                newName: "IX_FinancialYears_FinacialYear");
        }
    }
}
