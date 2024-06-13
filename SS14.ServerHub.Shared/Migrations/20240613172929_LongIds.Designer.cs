﻿// <auto-generated />
using System;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SS14.ServerHub.Shared.Data;

#nullable disable

namespace SS14.ServerHub.Shared.Migrations
{
    [DbContext(typeof(HubDbContext))]
    [Migration("20240613172929_LongIds")]
    partial class LongIds
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.AdvertisedServer", b =>
                {
                    b.Property<int>("AdvertisedServerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AdvertisedServerId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<IPAddress>("AdvertiserAddress")
                        .HasColumnType("inet");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string[]>("InferredTags")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<byte[]>("InfoData")
                        .HasColumnType("jsonb");

                    b.Property<byte[]>("StatusData")
                        .HasColumnType("jsonb");

                    b.HasKey("AdvertisedServerId");

                    b.HasIndex("Address")
                        .IsUnique();

                    b.ToTable("AdvertisedServer");

                    b.HasCheckConstraint("AddressSs14Uri", "\"Address\" LIKE 'ss14://%' OR \"Address\" LIKE 'ss14s://%'");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.HubAudit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("Actor")
                        .HasColumnType("uuid");

                    b.Property<JsonDocument>("Data")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Time");

                    b.ToTable("HubAudit");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.ServerStatusArchive", b =>
                {
                    b.Property<int>("AdvertisedServerId")
                        .HasColumnType("integer");

                    b.Property<long>("ServerStatusArchiveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ServerStatusArchiveId"));

                    b.Property<IPAddress>("AdvertiserAddress")
                        .HasColumnType("inet");

                    b.Property<string[]>("InferredTags")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<byte[]>("StatusData")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("AdvertisedServerId", "ServerStatusArchiveId");

                    b.ToTable("ServerStatusArchive");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.TrackedCommunity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsBanned")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TrackedCommunity");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.TrackedCommunityAddress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<ValueTuple<IPAddress, int>>("Address")
                        .HasColumnType("inet");

                    b.Property<int>("TrackedCommunityId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TrackedCommunityId");

                    b.ToTable("TrackedCommunityAddress");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.TrackedCommunityDomain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("DomainName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TrackedCommunityId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TrackedCommunityId");

                    b.ToTable("TrackedCommunityDomain");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.TrackedCommunityInfoMatch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Field")
                        .HasColumnType("integer");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("jsonpath");

                    b.Property<int>("TrackedCommunityId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TrackedCommunityId");

                    b.ToTable("TrackedCommunityInfoMatch");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.UniqueServerName", b =>
                {
                    b.Property<int>("AdvertisedServerId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("FirstSeen")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastSeen")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("AdvertisedServerId", "Name");

                    b.ToTable("UniqueServerName");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.ServerStatusArchive", b =>
                {
                    b.HasOne("SS14.ServerHub.Shared.Data.AdvertisedServer", "AdvertisedServer")
                        .WithMany()
                        .HasForeignKey("AdvertisedServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AdvertisedServer");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.TrackedCommunityAddress", b =>
                {
                    b.HasOne("SS14.ServerHub.Shared.Data.TrackedCommunity", "TrackedCommunity")
                        .WithMany("Addresses")
                        .HasForeignKey("TrackedCommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TrackedCommunity");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.TrackedCommunityDomain", b =>
                {
                    b.HasOne("SS14.ServerHub.Shared.Data.TrackedCommunity", "TrackedCommunity")
                        .WithMany("Domains")
                        .HasForeignKey("TrackedCommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TrackedCommunity");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.TrackedCommunityInfoMatch", b =>
                {
                    b.HasOne("SS14.ServerHub.Shared.Data.TrackedCommunity", "TrackedCommunity")
                        .WithMany("InfoMatches")
                        .HasForeignKey("TrackedCommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TrackedCommunity");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.UniqueServerName", b =>
                {
                    b.HasOne("SS14.ServerHub.Shared.Data.AdvertisedServer", "AdvertisedServer")
                        .WithMany()
                        .HasForeignKey("AdvertisedServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AdvertisedServer");
                });

            modelBuilder.Entity("SS14.ServerHub.Shared.Data.TrackedCommunity", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Domains");

                    b.Navigation("InfoMatches");
                });
#pragma warning restore 612, 618
        }
    }
}
