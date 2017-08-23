using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CC98.LogOn.Data;

namespace CC98.LogOn.Migrations
{
    [DbContext(typeof(CC98IdentityDbContext))]
    [Migration("20170814082539_AddAppInfo")]
    partial class AddAppInfo
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CC98.LogOn.Data.App", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AllowedScopesValue")
                        .HasColumnName("AllowedScopes");

                    b.Property<string>("Description");

                    b.Property<string>("DisplayName");

                    b.Property<int>("GrantTypes");

                    b.Property<bool>("IsEnabled");

                    b.Property<string>("LogoUri");

                    b.Property<string>("OwnerUserName");

                    b.Property<string>("RedirectUrisValue")
                        .HasColumnName("RedirectUris");

                    b.Property<Guid>("Secret");

                    b.Property<int>("State");

                    b.Property<string>("WebPageUri");

                    b.HasKey("Id");

                    b.ToTable("Apps");
                });

            modelBuilder.Entity("CC98.LogOn.Data.CC98User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
        }
    }
}
