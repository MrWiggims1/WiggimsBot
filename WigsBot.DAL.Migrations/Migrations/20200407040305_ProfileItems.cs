using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WigsBot.DAL.Migrations.Migrations
{
    public partial class ProfileItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uses",
                table: "RobbingItems");

            migrationBuilder.AddColumn<string>(
                name: "ItemJson",
                table: "Profiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemJson",
                table: "Profiles");

            migrationBuilder.AddColumn<int>(
                name: "Uses",
                table: "RobbingItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
