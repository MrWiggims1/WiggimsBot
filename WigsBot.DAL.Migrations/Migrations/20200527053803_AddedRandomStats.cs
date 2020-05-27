using Microsoft.EntityFrameworkCore.Migrations;

namespace WigsBot.DAL.Migrations.Migrations
{
    public partial class AddedRandomStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailiesCollected",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoldGainedFines",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoldLostFines",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoldLostFromTheft",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoldStolen",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RobbingAttackLost",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RobbingAttackWon",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RobbingDefendLost",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RobbingDefendWon",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RouletteFails",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RouletteSuccesses",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimesMimicked",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalDailyEarnings",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailiesCollected",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "GoldGainedFines",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "GoldLostFines",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "GoldLostFromTheft",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "GoldStolen",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "RobbingAttackLost",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "RobbingAttackWon",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "RobbingDefendLost",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "RobbingDefendWon",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "RouletteFails",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "RouletteSuccesses",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "TimesMimicked",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "TotalDailyEarnings",
                table: "Profiles");
        }
    }
}
