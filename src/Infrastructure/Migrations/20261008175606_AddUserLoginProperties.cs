using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserLoginProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                schema: "Users",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                schema: "Users",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EmailVerifiedAt",
                schema: "Users",
                table: "User",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastLoginAt",
                schema: "Users",
                table: "User",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                schema: "Users",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                schema: "Users",
                table: "User",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "Users",
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("01a10cab-98f1-7b35-aeae-96490d14578e"),
                columns: ["IsBlocked", "TwoFactorEnabled", "EmailVerifiedAt", "LastLoginAt", "FailedLoginAttempts", "LockoutEnd"],
                values: [false, false, null, null, 0, null]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerifiedAt",
                schema: "Users",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                schema: "Users",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                schema: "Users",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                schema: "Users",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                schema: "Users",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                schema: "Users",
                table: "User");
        }
    }
}
