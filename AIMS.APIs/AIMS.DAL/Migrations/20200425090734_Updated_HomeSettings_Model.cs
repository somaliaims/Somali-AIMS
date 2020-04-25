using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_HomeSettings_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AIMSBarTitle",
                table: "HomePageSettings",
                newName: "AIMSBarTitleText");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AIMSBarTitleText",
                table: "HomePageSettings",
                newName: "AIMSBarTitle");
        }
    }
}
