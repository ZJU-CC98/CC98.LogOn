using Microsoft.EntityFrameworkCore.Migrations;

namespace CC98.LogOn.Migrations
{
    public partial class AddApiResourceScope : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiResourceScopes",
                columns: table => new
                {
                    ApiId = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ScopeId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiResourceScopes");
        }
    }
}
