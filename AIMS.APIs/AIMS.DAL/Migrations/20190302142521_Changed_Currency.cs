using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Changed_Currency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EFCurrency",
                table: "EFCurrency");

            migrationBuilder.RenameTable(
                name: "EFCurrency",
                newName: "Currencies");

            migrationBuilder.RenameIndex(
                name: "IX_EFCurrency_Currency",
                table: "Currencies",
                newName: "IX_Currencies_Currency");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.RenameTable(
                name: "Currencies",
                newName: "EFCurrency");

            migrationBuilder.RenameIndex(
                name: "IX_Currencies_Currency",
                table: "EFCurrency",
                newName: "IX_EFCurrency_Currency");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EFCurrency",
                table: "EFCurrency",
                column: "Id");
        }
    }
}
