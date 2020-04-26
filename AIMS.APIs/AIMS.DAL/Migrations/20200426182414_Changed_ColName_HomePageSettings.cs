using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Changed_ColName_HomePageSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AIMSBarTitleText",
                table: "HomePageSettings",
                newName: "AIMSTitleBarText");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AIMSTitleBarText",
                table: "HomePageSettings",
                newName: "AIMSBarTitleText");
        }
    }
}
