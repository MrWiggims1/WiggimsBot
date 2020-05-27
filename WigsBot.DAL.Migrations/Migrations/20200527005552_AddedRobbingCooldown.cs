using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WigsBot.DAL.Migrations.Migrations
{
    public partial class AddedRobbingCooldown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RobbingCooldown",
                table: "Profiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RobbingCooldown",
                table: "Profiles");
        }
    }
}
