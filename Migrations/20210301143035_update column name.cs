using Microsoft.EntityFrameworkCore.Migrations;

namespace KeyLoggerApi.Migrations
{
    public partial class updatecolumnname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Keystroke",
                table: "WordLists",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Keystroke",
                table: "DetectedWords",
                newName: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "WordLists",
                newName: "Keystroke");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "DetectedWords",
                newName: "Keystroke");
        }
    }
}
