using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder?.AddColumn<int>(
                name: "RoleId",
                schema: "Users",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder?.CreateTable(
                name: "Role",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "Users",
                table: "Role",
                columns: ["Id", "Name"],
                values: new object[,]
                {
                    { 1, "SuperAdmin" },
                    { 2, "Admin" },
                    { 3, "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                schema: "Users",
                table: "User",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_RoleId",
                schema: "Users",
                table: "User",
                column: "RoleId",
                principalSchema: "Users",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder?.DropForeignKey(
                name: "FK_User_Role_RoleId",
                schema: "Users",
                table: "User");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "Users");

            migrationBuilder.DropIndex(
                name: "IX_User_RoleId",
                schema: "Users",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "Users",
                table: "User");
        }
    }
}
