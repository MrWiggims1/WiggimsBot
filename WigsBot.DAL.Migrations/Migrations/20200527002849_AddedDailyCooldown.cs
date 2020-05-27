using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WigsBot.DAL.Migrations.Migrations
{
    public partial class AddedDailyCooldown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DailyCooldown",
                table: "Profiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyCooldown",
                table: "Profiles");
        }
    }
}
