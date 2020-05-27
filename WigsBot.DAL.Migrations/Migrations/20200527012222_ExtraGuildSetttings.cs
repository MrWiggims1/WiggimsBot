using Microsoft.EntityFrameworkCore.Migrations;

namespace WigsBot.DAL.Migrations.Migrations
{
    public partial class ExtraGuildSetttings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoldPerLevelUp",
                table: "GuildPreferences",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsGoldEnabled",
                table: "GuildPreferences",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TimeoutRoleId",
                table: "GuildPreferences",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TimeoutTextChannelId",
                table: "GuildPreferences",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoldPerLevelUp",
                table: "GuildPreferences");

            migrationBuilder.DropColumn(
                name: "IsGoldEnabled",
                table: "GuildPreferences");

            migrationBuilder.DropColumn(
                name: "TimeoutRoleId",
                table: "GuildPreferences");

            migrationBuilder.DropColumn(
                name: "TimeoutTextChannelId",
                table: "GuildPreferences");
        }
    }
}
