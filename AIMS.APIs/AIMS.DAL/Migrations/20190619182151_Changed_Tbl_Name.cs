using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Changed_Tbl_Name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EFProjectImplementerRequests_Organizations_ImplementerId",
                table: "EFProjectImplementerRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_EFProjectImplementerRequests_Projects_ProjectId",
                table: "EFProjectImplementerRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EFProjectImplementerRequests",
                table: "EFProjectImplementerRequests");

            migrationBuilder.RenameTable(
                name: "EFProjectImplementerRequests",
                newName: "ImplementerRequests");

            migrationBuilder.RenameIndex(
                name: "IX_EFProjectImplementerRequests_ImplementerId",
                table: "ImplementerRequests",
                newName: "IX_ImplementerRequests_ImplementerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImplementerRequests",
                table: "ImplementerRequests",
                columns: new[] { "ProjectId", "ImplementerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ImplementerRequests_Organizations_ImplementerId",
                table: "ImplementerRequests",
                column: "ImplementerId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImplementerRequests_Projects_ProjectId",
                table: "ImplementerRequests",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImplementerRequests_Organizations_ImplementerId",
                table: "ImplementerRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ImplementerRequests_Projects_ProjectId",
                table: "ImplementerRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImplementerRequests",
                table: "ImplementerRequests");

            migrationBuilder.RenameTable(
                name: "ImplementerRequests",
                newName: "EFProjectImplementerRequests");

            migrationBuilder.RenameIndex(
                name: "IX_ImplementerRequests_ImplementerId",
                table: "EFProjectImplementerRequests",
                newName: "IX_EFProjectImplementerRequests_ImplementerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EFProjectImplementerRequests",
                table: "EFProjectImplementerRequests",
                columns: new[] { "ProjectId", "ImplementerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EFProjectImplementerRequests_Organizations_ImplementerId",
                table: "EFProjectImplementerRequests",
                column: "ImplementerId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EFProjectImplementerRequests_Projects_ProjectId",
                table: "EFProjectImplementerRequests",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
