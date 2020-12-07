using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Sublocations_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubLocations",
                table: "ProjectLocations",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Locations",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SubLocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubLocation = table.Column<string>(nullable: true),
                    LocationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Location",
                table: "Locations",
                column: "Location",
                unique: true,
                filter: "[Location] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SubLocations_LocationId",
                table: "SubLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SubLocations_SubLocation",
                table: "SubLocations",
                column: "SubLocation",
                unique: true,
                filter: "[SubLocation] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubLocations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_Location",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "SubLocations",
                table: "ProjectLocations");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
