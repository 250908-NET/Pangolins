using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pangolivia.API.Migrations
{
    /// <inheritdoc />
    public partial class init5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_records_users_host_user_id",
                table: "game_records");

            migrationBuilder.DropForeignKey(
                name: "FK_player_game_records_users_user_id",
                table: "player_game_records");

            migrationBuilder.DropForeignKey(
                name: "FK_quizzes_users_created_by_user_id",
                table: "quizzes");

            migrationBuilder.AddForeignKey(
                name: "FK_game_records_users_host_user_id",
                table: "game_records",
                column: "host_user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_player_game_records_users_user_id",
                table: "player_game_records",
                column: "user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_quizzes_users_created_by_user_id",
                table: "quizzes",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_records_users_host_user_id",
                table: "game_records");

            migrationBuilder.DropForeignKey(
                name: "FK_player_game_records_users_user_id",
                table: "player_game_records");

            migrationBuilder.DropForeignKey(
                name: "FK_quizzes_users_created_by_user_id",
                table: "quizzes");

            migrationBuilder.AddForeignKey(
                name: "FK_game_records_users_host_user_id",
                table: "game_records",
                column: "host_user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_player_game_records_users_user_id",
                table: "player_game_records",
                column: "user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_quizzes_users_created_by_user_id",
                table: "quizzes",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
