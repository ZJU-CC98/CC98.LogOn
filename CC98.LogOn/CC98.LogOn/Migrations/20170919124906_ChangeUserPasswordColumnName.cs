using Microsoft.EntityFrameworkCore.Migrations;

namespace CC98.LogOn.Migrations
{
    public partial class ChangeUserPasswordColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "User",
                newName: "UserPassword");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserPassword",
                table: "User",
                newName: "Password");
        }
    }
}
