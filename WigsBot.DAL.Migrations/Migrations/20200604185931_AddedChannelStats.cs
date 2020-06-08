using Microsoft.EntityFrameworkCore.Migrations;

namespace WigsBot.DAL.Migrations.Migrations
{
    public partial class AddedChannelStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminNotificationChannel",
                table: "GuildPreferences");

            migrationBuilder.DropColumn(
                name: "AdminRole",
                table: "GuildPreferences");

            migrationBuilder.DropColumn(
                name: "GuildEventNotification",
                table: "GuildPreferences");

            migrationBuilder.DropColumn(
                name: "PunishAtEveryone",
                table: "GuildPreferences");

            migrationBuilder.AddColumn<int>(
                name: "CommandsExecuted",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "StatChannelCatergoryId",
                table: "GuildPreferences",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalCommandsExecuted",
                table: "GuildPreferences",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StatChannels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelId = table.Column<decimal>(nullable: false),
                    StatOption = table.Column<int>(nullable: false),
                    StatMessage = table.Column<string>(maxLength: 12, nullable: false),
                    GuildPreferencesId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatChannels_GuildPreferences_GuildPreferencesId",
                        column: x => x.GuildPreferencesId,
                        principalTable: "GuildPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatChannels_GuildPreferencesId",
                table: "StatChannels",
                column: "GuildPreferencesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatChannels");

            migrationBuilder.DropColumn(
                name: "CommandsExecuted",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "StatChannelCatergoryId",
                table: "GuildPreferences");

            migrationBuilder.DropColumn(
                name: "TotalCommandsExecuted",
                table: "GuildPreferences");

            migrationBuilder.AddColumn<decimal>(
                name: "AdminNotificationChannel",
                table: "GuildPreferences",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AdminRole",
                table: "GuildPreferences",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "GuildEventNotification",
                table: "GuildPreferences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PunishAtEveryone",
                table: "GuildPreferences",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
