using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Amount_Funder_Disbursements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ProjectFunders",
                type: "decimal(11 ,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9 ,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ProjectDisbursements",
                type: "decimal(11, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9, 2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ProjectFunders",
                type: "decimal(9 ,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11 ,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ProjectDisbursements",
                type: "decimal(9, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11, 2)");
        }
    }
}
