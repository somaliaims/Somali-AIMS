using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_PK_Funders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectFunders",
                table: "ProjectFunders");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectFunders",
                table: "ProjectFunders",
                columns: new[] { "ProjectId", "FunderId", "GrantTypeId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectFunders",
                table: "ProjectFunders");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectFunders",
                table: "ProjectFunders",
                columns: new[] { "ProjectId", "FunderId" });
        }
    }
}
