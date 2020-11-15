using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_DatePosted_Links : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostedOn",
                table: "DocumentLinks");

            migrationBuilder.AddColumn<DateTime>(
                name: "DatePosted",
                table: "DocumentLinks",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatePosted",
                table: "DocumentLinks");

            migrationBuilder.AddColumn<DateTime>(
                name: "PostedOn",
                table: "DocumentLinks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
