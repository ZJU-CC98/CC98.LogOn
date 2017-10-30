using Microsoft.EntityFrameworkCore.Migrations;

namespace CC98.LogOn.Migrations
{
    public partial class AddAppInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GrantTypes",
                table: "Apps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OwnerUserName",
                table: "Apps",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Apps",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrantTypes",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "OwnerUserName",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Apps");
        }
    }
}
