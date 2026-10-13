using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInitialSuperUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Users",
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("01a10cab-98f1-7b35-aeae-96490d14578e"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Users",
                table: "User",
                columns: ["Id", "Email", "EmailVerifiedAt", "FailedLoginAttempts", "IsBlocked", "LastLoginAt", "LockoutEnd", "Password", "RoleId", "TwoFactorEnabled"],
                values: [new Guid("01a10cab-98f1-7b35-aeae-96490d14578e"), "super-admin@example.com", null, 0, false, null, null, "AQAAAAIAAYagAAAAEEyQ75ozi8VLY0iYz0IgFd2Jxr/ICs/6nlpojUmIJQ947Sybe428FBlk+Naizm+ZnQ==", 1, false]);
        }
    }
}
