using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSuperAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Users",
                table: "User",
                columns: ["Id", "Email", "Password", "RoleId"],
                values: [Guid.Parse("01a10cab-98f1-7b35-aeae-96490d14578e"), "super-admin@example.com", "AQAAAAIAAYagAAAAEEyQ75ozi8VLY0iYz0IgFd2Jxr/ICs/6nlpojUmIJQ947Sybe428FBlk+Naizm+ZnQ==", 1]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Users",
                table: "User",
                keyColumn: "Id",
                keyValue: Guid.Parse("01a10cab-98f1-7b35-aeae-96490d14578e"));
        }
    }
}
