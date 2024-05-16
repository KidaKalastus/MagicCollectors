using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicCollectors.Core.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFinishes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardFinish");

            migrationBuilder.DropTable(
                name: "Finishes");

            migrationBuilder.AddColumn<int>(
                name: "EtchedCount",
                table: "CollectionCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WantEtched",
                table: "CollectionCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EtchedFoil",
                table: "Cards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceUsdEtched",
                table: "Cards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EtchedCount",
                table: "CollectionCards");

            migrationBuilder.DropColumn(
                name: "WantEtched",
                table: "CollectionCards");

            migrationBuilder.DropColumn(
                name: "EtchedFoil",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "PriceUsdEtched",
                table: "Cards");

            migrationBuilder.CreateTable(
                name: "Finishes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardFinish",
                columns: table => new
                {
                    CardsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FinishesId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardFinish", x => new { x.CardsId, x.FinishesId });
                    table.ForeignKey(
                        name: "FK_CardFinish_Cards_CardsId",
                        column: x => x.CardsId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardFinish_Finishes_FinishesId",
                        column: x => x.FinishesId,
                        principalTable: "Finishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardFinish_FinishesId",
                table: "CardFinish",
                column: "FinishesId");
        }
    }
}
