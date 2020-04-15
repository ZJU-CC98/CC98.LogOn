using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CC98.LogOn.Migrations
{
    public partial class AddAccountLockDown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(nullable: false),
                    UserPassword = table.Column<string>(nullable: false),
                    RegMail = table.Column<string>(maxLength: 30, nullable: true),
                    Verified = table.Column<bool>(nullable: false),
                    face = table.Column<string>(nullable: true),
                    Sex = table.Column<int>(nullable: false),
                    adddate = table.Column<DateTime>(nullable: false),
                    lastlogin = table.Column<DateTime>(nullable: false),
                    article = table.Column<int>(nullable: false),
                    logins = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.UniqueConstraint("AK_User_UserName", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "UserTitles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    IconUri = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZjuAccountLockDownRecords",
                columns: table => new
                {
                    ZjuAccountId = table.Column<string>(maxLength: 50, nullable: false),
                    Time = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZjuAccountLockDownRecords", x => x.ZjuAccountId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "UserTitles");

            migrationBuilder.DropTable(
                name: "ZjuAccountLockDownRecords");
        }
    }
}
