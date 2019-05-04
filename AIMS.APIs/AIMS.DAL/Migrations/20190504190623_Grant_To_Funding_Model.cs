using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Grant_To_Funding_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectFunders_GrantTypes_GrantTypeId",
                table: "ProjectFunders");

            migrationBuilder.DropTable(
                name: "GrantTypes");

            migrationBuilder.RenameColumn(
                name: "GrantTypeId",
                table: "ProjectFunders",
                newName: "FundingTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectFunders_GrantTypeId",
                table: "ProjectFunders",
                newName: "IX_ProjectFunders_FundingTypeId");

            migrationBuilder.CreateTable(
                name: "FundingTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FundingType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundingTypes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectFunders_FundingTypes_FundingTypeId",
                table: "ProjectFunders",
                column: "FundingTypeId",
                principalTable: "FundingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectFunders_FundingTypes_FundingTypeId",
                table: "ProjectFunders");

            migrationBuilder.DropTable(
                name: "FundingTypes");

            migrationBuilder.RenameColumn(
                name: "FundingTypeId",
                table: "ProjectFunders",
                newName: "GrantTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectFunders_FundingTypeId",
                table: "ProjectFunders",
                newName: "IX_ProjectFunders_GrantTypeId");

            migrationBuilder.CreateTable(
                name: "GrantTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GrantType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrantTypes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectFunders_GrantTypes_GrantTypeId",
                table: "ProjectFunders",
                column: "GrantTypeId",
                principalTable: "GrantTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
