﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WigsBot.DAL;

namespace WigsBot.DAL.Migrations.Migrations
{
    [DbContext(typeof(RPGContext))]
    [Migration("20200527002849_AddedDailyCooldown")]
    partial class AddedDailyCooldown
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WigsBot.DAL.Models.GuildPreferences.GuildPreferences", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("AdminNotificationChannel")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("AdminRole")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("AssignableRoleJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("AutoRole")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("ErrorListLength")
                        .HasColumnType("int");

                    b.Property<bool>("GuildEventNotification")
                        .HasColumnType("bit");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<bool>("PunishAtEveryone")
                        .HasColumnType("bit");

                    b.Property<bool>("SpellingEnabled")
                        .HasColumnType("bit");

                    b.Property<int>("XpPerMessage")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("GuildPreferences");
                });

            modelBuilder.Entity("WigsBot.DAL.Models.Items.RobbingItems", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("AllowHeist")
                        .HasColumnType("bit");

                    b.Property<decimal>("AttackBuff")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Cost")
                        .HasColumnType("int");

                    b.Property<decimal>("DefenseBuff")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LvlRequired")
                        .HasColumnType("int");

                    b.Property<int>("MaxAllowed")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RobbingItems");
                });

            modelBuilder.Entity("WigsBot.DAL.Models.Profiles.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BattleNetUsername")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("BeatSaberId")
                        .HasColumnType("bigint");

                    b.Property<int>("BoganCount")
                        .HasColumnType("int");

                    b.Property<string>("CODMWUsername")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DailyCooldown")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("DiscordId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("Gold")
                        .HasColumnType("int");

                    b.Property<int>("Gots")
                        .HasColumnType("int");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<bool>("IsMimicable")
                        .HasColumnType("bit");

                    b.Property<string>("ItemJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LeaveCount")
                        .HasColumnType("int");

                    b.Property<bool>("QuietMode")
                        .HasColumnType("bit");

                    b.Property<int>("SpellCorrectCount")
                        .HasColumnType("int");

                    b.Property<int>("SpellErrorCount")
                        .HasColumnType("int");

                    b.Property<string>("SpellErrorList")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.Property<string>("ToDoJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UplayUsername")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Xp")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Profiles");
                });
#pragma warning restore 612, 618
        }
    }
}