﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using temsAPI.Data;

namespace temsAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210116190849_AddingEquipmentEntities")]
    partial class AddingEquipmentEntities
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

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

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
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

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.DataType", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("DataTypes");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.Equipment", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Commentary")
                        .HasColumnType("float");

                    b.Property<DateTime>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDefect")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<string>("ParentID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TEMSID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("ParentID");

                    b.HasIndex("SerialNumber");

                    b.HasIndex("TEMSID");

                    b.ToTable("Equipments");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.EquipmentDefinition", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("EquipmentTypeID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("EquipmentTypeID");

                    b.HasIndex("Identifier");

                    b.ToTable("EquipmentDefinitions");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.EquipmentDefinitionKinship", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ChildDefinitionID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ParentDefinitionID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("ChildDefinitionID");

                    b.HasIndex("ParentDefinitionID");

                    b.ToTable("EquipmentDefinitionKinships");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.EquipmentSpecifications", b =>
                {
                    b.Property<string>("EquipmentTypeID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("EquipmentTypeID1")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PropertyID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EquipmentTypeID");

                    b.HasIndex("EquipmentTypeID");

                    b.HasIndex("EquipmentTypeID1");

                    b.HasIndex("PropertyID");

                    b.ToTable("EquipmentSpecifications");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.EquipmentType", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ParentEquipmentTypeID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("ParentEquipmentTypeID");

                    b.ToTable("EquipmentTypes");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.Property", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DataTypeID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("DataTypeID");

                    b.ToTable("Properties");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.PropertyEquipmentTypeAssociation", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PropertyID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TypeID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("PropertyID");

                    b.HasIndex("TypeID");

                    b.ToTable("PropertyEquipmentTypeAssociations");
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
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
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

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.Equipment", b =>
                {
                    b.HasOne("temsAPI.Data.Entities.EquipmentEntities.Equipment", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentID");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.EquipmentDefinition", b =>
                {
                    b.HasOne("temsAPI.Data.Entities.EquipmentEntities.EquipmentType", "EquipmentType")
                        .WithMany()
                        .HasForeignKey("EquipmentTypeID");

                    b.Navigation("EquipmentType");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.EquipmentDefinitionKinship", b =>
                {
                    b.HasOne("temsAPI.Data.Entities.EquipmentEntities.EquipmentDefinition", "ChildDefinition")
                        .WithMany()
                        .HasForeignKey("ChildDefinitionID");

                    b.HasOne("temsAPI.Data.Entities.EquipmentEntities.EquipmentDefinition", "ParentDefinition")
                        .WithMany()
                        .HasForeignKey("ParentDefinitionID");

                    b.Navigation("ChildDefinition");

                    b.Navigation("ParentDefinition");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.EquipmentSpecifications", b =>
                {
                    b.HasOne("temsAPI.Data.Entities.EquipmentEntities.EquipmentType", "EquipmentType")
                        .WithMany()
                        .HasForeignKey("EquipmentTypeID1");

                    b.HasOne("temsAPI.Data.Entities.EquipmentEntities.Property", "Property")
                        .WithMany()
                        .HasForeignKey("PropertyID");

                    b.Navigation("EquipmentType");

                    b.Navigation("Property");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.EquipmentType", b =>
                {
                    b.HasOne("temsAPI.Data.Entities.EquipmentEntities.EquipmentType", "ParentEquipmentType")
                        .WithMany()
                        .HasForeignKey("ParentEquipmentTypeID");

                    b.Navigation("ParentEquipmentType");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.Property", b =>
                {
                    b.HasOne("temsAPI.Data.Entities.EquipmentEntities.DataType", "DataType")
                        .WithMany()
                        .HasForeignKey("DataTypeID");

                    b.Navigation("DataType");
                });

            modelBuilder.Entity("temsAPI.Data.Entities.EquipmentEntities.PropertyEquipmentTypeAssociation", b =>
                {
                    b.HasOne("temsAPI.Data.Entities.EquipmentEntities.Property", "Property")
                        .WithMany()
                        .HasForeignKey("PropertyID");

                    b.HasOne("temsAPI.Data.Entities.EquipmentEntities.EquipmentType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeID");

                    b.Navigation("Property");

                    b.Navigation("Type");
                });
#pragma warning restore 612, 618
        }
    }
}
