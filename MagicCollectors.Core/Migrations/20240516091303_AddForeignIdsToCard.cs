using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicCollectors.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignIdsToCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CardMarketId",
                table: "Cards",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OracleId",
                table: "Cards",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TcgPlayerId",
                table: "Cards",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardMarketId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "OracleId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "TcgPlayerId",
                table: "Cards");
        }
    }
}
