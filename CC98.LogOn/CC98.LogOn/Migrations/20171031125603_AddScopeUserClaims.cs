using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CC98.LogOn.Migrations
{
    public partial class AddScopeUserClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "face",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegMail",
                table: "User",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserClaims",
                table: "AppScopes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Verified",
                table: "User");

            migrationBuilder.DropColumn(
                name: "face",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RegMail",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserClaims",
                table: "AppScopes");
        }
    }
}
