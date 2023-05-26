﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;
using ShoppingMallSys.DataBase;

#nullable disable

namespace ShoppingMallSys.Migrations
{
    [DbContext(typeof(DaoDbContext))]
    [Migration("20230505122516_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("NETCORE")
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ShoppingMallSys.Models.Student", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("NVARCHAR2(450)")
                        .HasColumnName("USERID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("NAME");

                    b.HasKey("UserId");

                    b.ToTable("STUDENT", "NETCORE");
                });
#pragma warning restore 612, 618
        }
    }
}