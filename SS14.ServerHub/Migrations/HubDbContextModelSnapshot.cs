﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SS14.ServerHub.Data;

#nullable disable

namespace SS14.ServerHub.Migrations
{
    [DbContext(typeof(HubDbContext))]
    partial class HubDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SS14.ServerHub.Data.AdvertisedServer", b =>
                {
                    b.Property<int>("AdvertisedServerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AdvertisedServerId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("Secret")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("AdvertisedServerId");

                    b.HasIndex("Address")
                        .IsUnique();

                    b.HasIndex("Secret")
                        .IsUnique();

                    b.ToTable("AdvertisedServer");

                    b.HasCheckConstraint("AddressSs14Uri", "\"Address\" LIKE 'ss14://%' OR \"Address\" LIKE 'ss14s://%'");
                });
#pragma warning restore 612, 618
        }
    }
}
