using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class User_Model_Changed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "DeActivatedOn",
                table: "Users",
                newName: "LastLogin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastLogin",
                table: "Users",
                newName: "DeActivatedOn");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }
    }
}
