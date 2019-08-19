using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Decimal_Precision_ExpectedDisbursements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedDisbursements",
                table: "ExpectedDisbursements",
                type: "decimal(11, 2)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedDisbursements",
                table: "ExpectedDisbursements",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11, 2)");
        }
    }
}
