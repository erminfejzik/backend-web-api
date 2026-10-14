using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserTokenRemoveUnusedIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserToken_UserId_Purpose",
                schema: "Users",
                table: "UserToken");

            migrationBuilder.CreateIndex(
                name: "IX_UserToken_UserId",
                schema: "Users",
                table: "UserToken",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserToken_UserId",
                schema: "Users",
                table: "UserToken");

            migrationBuilder.CreateIndex(
                name: "IX_UserToken_UserId_Purpose",
                schema: "Users",
                table: "UserToken",
                columns: ["UserId", "Purpose"]);
        }
    }
}
