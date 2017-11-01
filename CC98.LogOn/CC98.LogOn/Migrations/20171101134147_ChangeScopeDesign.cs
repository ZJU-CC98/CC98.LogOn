using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CC98.LogOn.Migrations
{
    public partial class ChangeScopeDesign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiResourceScopes");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "AppScopes");

            migrationBuilder.DropColumn(
                name: "Claims",
                table: "ApiResources");

            migrationBuilder.AddColumn<string>(
                name: "ApiId",
                table: "AppScopes",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "AppScopes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AppScopes_ApiId",
                table: "AppScopes",
                column: "ApiId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppScopes_ApiResources_ApiId",
                table: "AppScopes",
                column: "ApiId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppScopes_ApiResources_ApiId",
                table: "AppScopes");

            migrationBuilder.DropIndex(
                name: "IX_AppScopes_ApiId",
                table: "AppScopes");

            migrationBuilder.DropColumn(
                name: "ApiId",
                table: "AppScopes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AppScopes");

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "AppScopes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Claims",
                table: "ApiResources",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApiResourceScopes",
                columns: table => new
                {
                    ApiId = table.Column<string>(nullable: false),
                    ScopeId = table.Column<string>(nullable: false),
                    IsRequired = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResourceScopes", x => new { x.ApiId, x.ScopeId });
                    table.ForeignKey(
                        name: "FK_ApiResourceScopes_ApiResources_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiResourceScopes_AppScopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "AppScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceScopes_ScopeId",
                table: "ApiResourceScopes",
                column: "ScopeId");
        }
    }
}
