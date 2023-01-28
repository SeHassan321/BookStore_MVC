using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UISCOM.Migrations
{
    public partial class removeAuthorFromBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authers_AutherId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_AutherId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "AutherId",
                table: "Books");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AutherId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_AutherId",
                table: "Books",
                column: "AutherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authers_AutherId",
                table: "Books",
                column: "AutherId",
                principalTable: "Authers",
                principalColumn: "Id");
        }
    }
}
