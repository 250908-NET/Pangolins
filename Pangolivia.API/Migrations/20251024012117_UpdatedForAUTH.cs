using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pangolivia.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedForAUTH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "auth_uuid",
                table: "users",
                newName: "ProfileImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "auth_sub",
                table: "users",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_users_auth_sub",
                table: "users",
                column: "auth_sub",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_auth_sub",
                table: "users");

            migrationBuilder.DropColumn(
                name: "auth_sub",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "users",
                newName: "auth_uuid");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
