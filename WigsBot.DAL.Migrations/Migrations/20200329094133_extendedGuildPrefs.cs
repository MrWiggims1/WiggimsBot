using Microsoft.EntityFrameworkCore.Migrations;

namespace WigsBot.DAL.Migrations.Migrations
{
    public partial class extendedGuildPrefs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AdminNotificationChannel",
                table: "GuildPreferences",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AutoRole",
                table: "GuildPreferences",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "GuildEventNotification",
                table: "GuildPreferences",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PunishAtEveryone",
                table: "GuildPreferences",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminNotificationChannel",
                table: "GuildPreferences");

            migrationBuilder.DropColumn(
                name: "AutoRole",
                table: "GuildPreferences");

            migrationBuilder.DropColumn(
                name: "GuildEventNotification",
                table: "GuildPreferences");

            migrationBuilder.DropColumn(
                name: "PunishAtEveryone",
                table: "GuildPreferences");
        }
    }
}
