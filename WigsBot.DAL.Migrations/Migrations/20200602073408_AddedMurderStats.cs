using Microsoft.EntityFrameworkCore.Migrations;

namespace WigsBot.DAL.Migrations.Migrations
{
    public partial class AddedMurderStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimesBeenMurdered",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimesEscapedMurder",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimesFailedToMurder",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimesMurdered",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimesBeenMurdered",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "TimesEscapedMurder",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "TimesFailedToMurder",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "TimesMurdered",
                table: "Profiles");
        }
    }
}
