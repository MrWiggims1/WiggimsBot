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
    [Migration("20200606095917_fixchannelstatFK2")]
    partial class fixchannelstatFK2
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

                    b.Property<string>("AssignableRoleJson")
                        .HasColumnType("nvarchar(2000)")
                        .HasMaxLength(2000);

                    b.Property<decimal>("AutoRole")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("ErrorListLength")
                        .HasColumnType("int");

                    b.Property<int>("GoldPerLevelUp")
                        .HasColumnType("int");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<bool>("IsGoldEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("SpellingEnabled")
                        .HasColumnType("bit");

                    b.Property<decimal>("StatChannelCatergoryId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("TimeoutRoleId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("TimeoutTextChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("TotalCommandsExecuted")
                        .HasColumnType("int");

                    b.Property<int>("XpPerMessage")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("GuildPreferences");
                });

            modelBuilder.Entity("WigsBot.DAL.Models.GuildPreferences.StatChannel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("GuildPreferencesId")
                        .HasColumnType("int");

                    b.Property<string>("StatMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(12)")
                        .HasMaxLength(12);

                    b.Property<int>("StatOption")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GuildPreferencesId");

                    b.ToTable("StatChannels");
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
                        .IsRequired()
                        .HasColumnType("nvarchar(300)")
                        .HasMaxLength(300);

                    b.Property<int>("LvlRequired")
                        .HasColumnType("int");

                    b.Property<int>("MaxAllowed")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(40)")
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.ToTable("RobbingItems");
                });

            modelBuilder.Entity("WigsBot.DAL.Models.Profiles.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("BeatSaberId")
                        .HasColumnType("bigint");

                    b.Property<int>("BoganCount")
                        .HasColumnType("int");

                    b.Property<int>("CoinFilpsWon")
                        .HasColumnType("int");

                    b.Property<int>("CoindFlipsLost")
                        .HasColumnType("int");

                    b.Property<int>("CommandsExecuted")
                        .HasColumnType("int");

                    b.Property<int>("DailiesCollected")
                        .HasColumnType("int");

                    b.Property<DateTime>("DailyCooldown")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("DiscordId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("Gold")
                        .HasColumnType("int");

                    b.Property<int>("GoldGainedFines")
                        .HasColumnType("int");

                    b.Property<int>("GoldLostCoinFlip")
                        .HasColumnType("int");

                    b.Property<int>("GoldLostFines")
                        .HasColumnType("int");

                    b.Property<int>("GoldLostFromTheft")
                        .HasColumnType("int");

                    b.Property<int>("GoldPayedToMembers")
                        .HasColumnType("int");

                    b.Property<int>("GoldRecivedFromMembers")
                        .HasColumnType("int");

                    b.Property<int>("GoldStolen")
                        .HasColumnType("int");

                    b.Property<int>("GoldWonCoinFlip")
                        .HasColumnType("int");

                    b.Property<int>("Gots")
                        .HasColumnType("int");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<bool>("IsMimicable")
                        .HasColumnType("bit");

                    b.Property<string>("ItemJson")
                        .HasColumnType("nvarchar(500)")
                        .HasMaxLength(500);

                    b.Property<int>("LeaveCount")
                        .HasColumnType("int");

                    b.Property<bool>("QuietMode")
                        .HasColumnType("bit");

                    b.Property<int>("RobbingAttackLost")
                        .HasColumnType("int");

                    b.Property<int>("RobbingAttackWon")
                        .HasColumnType("int");

                    b.Property<DateTime>("RobbingCooldown")
                        .HasColumnType("datetime2");

                    b.Property<int>("RobbingDefendLost")
                        .HasColumnType("int");

                    b.Property<int>("RobbingDefendWon")
                        .HasColumnType("int");

                    b.Property<int>("RouletteFails")
                        .HasColumnType("int");

                    b.Property<int>("RouletteSuccesses")
                        .HasColumnType("int");

                    b.Property<int>("SpellCorrectCount")
                        .HasColumnType("int");

                    b.Property<int>("SpellErrorCount")
                        .HasColumnType("int");

                    b.Property<string>("SpellErrorList")
                        .HasColumnType("nvarchar(600)")
                        .HasMaxLength(600);

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.Property<int>("TimesBeenMurdered")
                        .HasColumnType("int");

                    b.Property<int>("TimesEscapedMurder")
                        .HasColumnType("int");

                    b.Property<int>("TimesFailedToMurder")
                        .HasColumnType("int");

                    b.Property<int>("TimesMimicked")
                        .HasColumnType("int");

                    b.Property<int>("TimesMurdered")
                        .HasColumnType("int");

                    b.Property<int>("TimesPayedByMember")
                        .HasColumnType("int");

                    b.Property<int>("TimesPayedOtherMember")
                        .HasColumnType("int");

                    b.Property<string>("ToDoJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalDailyEarnings")
                        .HasColumnType("int");

                    b.Property<int>("TotalRouletteEarnings")
                        .HasColumnType("int");

                    b.Property<string>("UplayUsername")
                        .HasColumnType("nvarchar(40)")
                        .HasMaxLength(40);

                    b.Property<int>("Xp")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("WigsBot.DAL.Models.GuildPreferences.StatChannel", b =>
                {
                    b.HasOne("WigsBot.DAL.Models.GuildPreferences.GuildPreferences", "GuildPreferences")
                        .WithMany("StatChannels")
                        .HasForeignKey("GuildPreferencesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
