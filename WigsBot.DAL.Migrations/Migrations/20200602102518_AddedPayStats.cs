using Microsoft.EntityFrameworkCore.Migrations;

namespace WigsBot.DAL.Migrations.Migrations
{
    public partial class AddedPayStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoldPayedToMembers",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoldRecivedFromMembers",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimesPayedByMember",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimesPayedOtherMember",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoldPayedToMembers",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "GoldRecivedFromMembers",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "TimesPayedByMember",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "TimesPayedOtherMember",
                table: "Profiles");
        }
    }
}
