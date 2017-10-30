using Microsoft.EntityFrameworkCore.Migrations;

namespace CC98.LogOn.Migrations
{
    public partial class AddAllowedCors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowedCorsOrigins",
                table: "Apps",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedCorsOrigins",
                table: "Apps");
        }
    }
}
