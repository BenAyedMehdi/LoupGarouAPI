using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LoupGarou.Migrations
{
    /// <inheritdoc />
    public partial class SeedCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cards",
                columns: new[] { "CardId", "CardName", "Description", "ImageName" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Werewolf", "Werewolves work together to kill all the village. They choose a player to kill every night, and they pretend to be villagers during the day.", "werewolf" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Villager", "This role has no superpowers, sleeping all night and only votes during the day.", "villager" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Protector", "The protector chooses a player to protect from the werewolves every night. He can protect himself, and he cannot protect the same player twice in a row.", "protector" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Witch", "The witch has two potions: one that can heal the werewolves' target, the other that can kill any player.", "witch" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "CardId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "CardId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "CardId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "CardId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));
        }
    }
}
