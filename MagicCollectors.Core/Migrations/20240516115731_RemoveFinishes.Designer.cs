﻿// <auto-generated />
using System;
using MagicCollectors.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MagicCollectors.Core.Migrations
{
    [DbContext(typeof(MagicCollectorsDbContext))]
    [Migration("20240516115731_RemoveFinishes")]
    partial class RemoveFinishes
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CardFrameEffect", b =>
                {
                    b.Property<Guid>("CardsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("FrameEffectsId")
                        .HasColumnType("bigint");

                    b.HasKey("CardsId", "FrameEffectsId");

                    b.HasIndex("FrameEffectsId");

                    b.ToTable("CardFrameEffect");
                });

            modelBuilder.Entity("CardPromoType", b =>
                {
                    b.Property<Guid>("CardsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("PromoTypesId")
                        .HasColumnType("bigint");

                    b.HasKey("CardsId", "PromoTypesId");

                    b.HasIndex("PromoTypesId");

                    b.ToTable("CardPromoType");
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.Card", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BorderColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("CardMarketId")
                        .HasColumnType("bigint");

                    b.Property<string>("CollectorNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("ConvertedManaCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("EtchedFoil")
                        .HasColumnType("bit");

                    b.Property<bool>("Extra")
                        .HasColumnType("bit");

                    b.Property<string>("FlavorText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Foil")
                        .HasColumnType("bit");

                    b.Property<string>("ImageDetails")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ManaCost")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("NonFoil")
                        .HasColumnType("bit");

                    b.Property<Guid?>("OracleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OracleText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Oversized")
                        .HasColumnType("bit");

                    b.Property<decimal>("PriceEuro")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PriceEuroFoil")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PriceTix")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PriceUsd")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PriceUsdEtched")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PriceUsdFoil")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("Promo")
                        .HasColumnType("bit");

                    b.Property<int>("Rarity")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("SetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SpellType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("TcgPlayerId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("SetId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.CollectionCard", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("CardId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<int>("EtchedCount")
                        .HasColumnType("int");

                    b.Property<int>("FoilCount")
                        .HasColumnType("int");

                    b.Property<int>("Want")
                        .HasColumnType("int");

                    b.Property<int>("WantEtched")
                        .HasColumnType("int");

                    b.Property<int>("WantFoil")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("CardId");

                    b.ToTable("CollectionCards");
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.CollectionSet", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("CostOfMissingCards")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<int>("Missing")
                        .HasColumnType("int");

                    b.Property<Guid>("SetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("ValueOfOwnedCards")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Want")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("SetId");

                    b.ToTable("CollectionSets");
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.FrameEffect", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("FrameEffects");
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.PromoType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PromoTypes");
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.Set", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Block")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BlockCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CardCount")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Digital")
                        .HasColumnType("bit");

                    b.Property<bool>("FoilOnly")
                        .HasColumnType("bit");

                    b.Property<string>("IconSvgUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("NonFoilOnly")
                        .HasColumnType("bit");

                    b.Property<string>("ParentSetCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Sets");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("CardFrameEffect", b =>
                {
                    b.HasOne("MagicCollectors.Core.Model.Card", null)
                        .WithMany()
                        .HasForeignKey("CardsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MagicCollectors.Core.Model.FrameEffect", null)
                        .WithMany()
                        .HasForeignKey("FrameEffectsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CardPromoType", b =>
                {
                    b.HasOne("MagicCollectors.Core.Model.Card", null)
                        .WithMany()
                        .HasForeignKey("CardsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MagicCollectors.Core.Model.PromoType", null)
                        .WithMany()
                        .HasForeignKey("PromoTypesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.Card", b =>
                {
                    b.HasOne("MagicCollectors.Core.Model.Set", "Set")
                        .WithMany("Cards")
                        .HasForeignKey("SetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Set");
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.CollectionCard", b =>
                {
                    b.HasOne("MagicCollectors.Core.Model.ApplicationUser", null)
                        .WithMany("CollectionCards")
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("MagicCollectors.Core.Model.Card", "Card")
                        .WithMany("CollectionCards")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.CollectionSet", b =>
                {
                    b.HasOne("MagicCollectors.Core.Model.ApplicationUser", null)
                        .WithMany("CollectionSets")
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("MagicCollectors.Core.Model.Set", "Set")
                        .WithMany("CollectionSets")
                        .HasForeignKey("SetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Set");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MagicCollectors.Core.Model.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MagicCollectors.Core.Model.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MagicCollectors.Core.Model.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("MagicCollectors.Core.Model.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.ApplicationUser", b =>
                {
                    b.Navigation("CollectionCards");

                    b.Navigation("CollectionSets");
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.Card", b =>
                {
                    b.Navigation("CollectionCards");
                });

            modelBuilder.Entity("MagicCollectors.Core.Model.Set", b =>
                {
                    b.Navigation("Cards");

                    b.Navigation("CollectionSets");
                });
#pragma warning restore 612, 618
        }
    }
}
