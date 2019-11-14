using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_capacity_for_numbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ProjectValue",
                table: "Projects",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DisbursementType",
                table: "ProjectDisbursements",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ProjectDisbursements",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "EnvelopeYearlyBreakups",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11, 2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ProjectValue",
                table: "Projects",
                type: "decimal(11, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DisbursementType",
                table: "ProjectDisbursements",
                type: "decimal(11, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ProjectDisbursements",
                type: "decimal(11, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "EnvelopeYearlyBreakups",
                type: "decimal(11, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");
        }
    }
}
