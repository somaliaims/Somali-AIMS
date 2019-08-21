using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Homepage_Updated_Col_Names : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IntroductoryParagraph",
                table: "HomePageSettings",
                newName: "IntroductionText");

            migrationBuilder.RenameColumn(
                name: "IntroductoryHeading",
                table: "HomePageSettings",
                newName: "IntroductionHeading");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IntroductionText",
                table: "HomePageSettings",
                newName: "IntroductoryParagraph");

            migrationBuilder.RenameColumn(
                name: "IntroductionHeading",
                table: "HomePageSettings",
                newName: "IntroductoryHeading");
        }
    }
}
