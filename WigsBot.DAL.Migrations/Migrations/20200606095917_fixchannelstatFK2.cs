using Microsoft.EntityFrameworkCore.Migrations;

namespace WigsBot.DAL.Migrations.Migrations
{
    public partial class fixchannelstatFK2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatChannels_GuildPreferences_GuildPreferencesId",
                table: "StatChannels");

            migrationBuilder.AlterColumn<int>(
                name: "GuildPreferencesId",
                table: "StatChannels",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StatChannels_GuildPreferences_GuildPreferencesId",
                table: "StatChannels",
                column: "GuildPreferencesId",
                principalTable: "GuildPreferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatChannels_GuildPreferences_GuildPreferencesId",
                table: "StatChannels");

            migrationBuilder.AlterColumn<int>(
                name: "GuildPreferencesId",
                table: "StatChannels",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_StatChannels_GuildPreferences_GuildPreferencesId",
                table: "StatChannels",
                column: "GuildPreferencesId",
                principalTable: "GuildPreferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
