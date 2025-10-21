using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pangolivia.API.Migrations
{
    /// <inheritdoc />
    public partial class Updates_10_21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "score",
                table: "player_game_records",
                newName: "Score");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Score",
                table: "player_game_records",
                newName: "score");
        }
    }
}
