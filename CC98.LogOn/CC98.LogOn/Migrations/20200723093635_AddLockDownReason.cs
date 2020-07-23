using Microsoft.EntityFrameworkCore.Migrations;

namespace CC98.LogOn.Migrations
{
    public partial class AddLockDownReason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ZjuAccountLockDownRecords",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ZjuAccountLockDownRecords");
        }
    }
}
