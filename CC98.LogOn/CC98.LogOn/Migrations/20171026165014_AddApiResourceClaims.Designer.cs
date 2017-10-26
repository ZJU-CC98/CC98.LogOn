﻿// <auto-generated />
using CC98.LogOn.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace CC98.LogOn.Migrations
{
    [DbContext(typeof(CC98IdentityDbContext))]
    [Migration("20171026165014_AddApiResourceClaims")]
    partial class AddApiResourceClaims
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CC98.LogOn.Data.App", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AllowedCorsOriginsValue")
                        .HasColumnName("AllowedCorsOrigins");

                    b.Property<string>("AllowedScopesValue")
                        .HasColumnName("AllowedScopes");

                    b.Property<DateTimeOffset>("CreateTime");

                    b.Property<string>("Description");

                    b.Property<string>("DisplayName");

                    b.Property<int>("GrantTypes");

                    b.Property<bool>("IsEnabled");

                    b.Property<string>("LogoUri");

                    b.Property<string>("OwnerUserName");

                    b.Property<string>("PostLogoutRedirectUrisValue")
                        .HasColumnName("PostLogoutRedirectUris");

                    b.Property<string>("RedirectUrisValue")
                        .HasColumnName("RedirectUris");

                    b.Property<Guid>("Secret");

                    b.Property<int>("State");

                    b.Property<string>("WebPageUri");

                    b.HasKey("Id");

                    b.ToTable("Apps");
                });

            modelBuilder.Entity("CC98.LogOn.Data.AppApiResource", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("ClaimsValue")
                        .HasColumnName("Claims");

                    b.Property<string>("Description");

                    b.Property<string>("DisplayName")
                        .IsRequired();

                    b.Property<Guid>("Secret");

                    b.HasKey("Id");

                    b.ToTable("ApiResources");
                });

            modelBuilder.Entity("CC98.LogOn.Data.AppScope", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20);

                    b.Property<string>("Description");

                    b.Property<string>("DisplayName")
                        .IsRequired();

                    b.Property<bool>("IsHidden");

                    b.Property<string>("Region");

                    b.HasKey("Id");

                    b.ToTable("AppScopes");
                });

            modelBuilder.Entity("CC98.LogOn.Data.CC98Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("Description");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Region");

                    b.HasKey("Id");

                    b.HasAlternateKey("Name");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("CC98.LogOn.Data.CC98User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UserId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("UserName");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnName("UserPassword");

                    b.HasKey("Id");

                    b.HasAlternateKey("Name");

                    b.ToTable("User");
                });

            modelBuilder.Entity("CC98.LogOn.Data.CC98UserRole", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("CC98.LogOn.Data.CC98UserRole", b =>
                {
                    b.HasOne("CC98.LogOn.Data.CC98Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CC98.LogOn.Data.CC98User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
