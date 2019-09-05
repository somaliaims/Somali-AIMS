using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Disbursement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ProjectDisbursements",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11, 2)");

            migrationBuilder.AddColumn<decimal>(
                name: "DisbursementType",
                table: "ProjectDisbursements",
                type: "decimal(11, 2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisbursementType",
                table: "ProjectDisbursements");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ProjectDisbursements",
                type: "decimal(11, 2)",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
