using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pangolivia.API.Migrations
{
    /// <inheritdoc />
    public partial class init6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_player_game_records_users_user_id",
                table: "player_game_records");

            migrationBuilder.AddForeignKey(
                name: "FK_player_game_records_users_user_id",
                table: "player_game_records",
                column: "user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_player_game_records_users_user_id",
                table: "player_game_records");

            migrationBuilder.AddForeignKey(
                name: "FK_player_game_records_users_user_id",
                table: "player_game_records",
                column: "user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
