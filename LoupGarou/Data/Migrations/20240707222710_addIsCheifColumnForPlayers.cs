using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoupGarou.Migrations
{
    /// <inheritdoc />
    public partial class addIsCheifColumnForPlayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCheif",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCheif",
                table: "Players");
        }
    }
}
