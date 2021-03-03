using Microsoft.EntityFrameworkCore.Migrations;

namespace KeyLoggerApi.Migrations
{
    public partial class setkeyloggerkeystrokecolumntonvarcharmax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Keystroke",
                table: "Keyloggers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Keystroke",
                table: "Keyloggers",
                type: "nvarchar(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
