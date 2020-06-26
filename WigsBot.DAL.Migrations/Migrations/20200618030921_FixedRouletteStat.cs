using Microsoft.EntityFrameworkCore.Migrations;

namespace WigsBot.DAL.Migrations.Migrations
{
    public partial class FixedRouletteStat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommandsExecuted",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "TotalRouletteEarnings",
                table: "Profiles");

            migrationBuilder.AddColumn<int>(
                name: "GoldLostRoulette",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoldWonRoulette",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoldLostRoulette",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "GoldWonRoulette",
                table: "Profiles");

            migrationBuilder.AddColumn<int>(
                name: "CommandsExecuted",
                table: "Profiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRouletteEarnings",
                table: "Profiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
