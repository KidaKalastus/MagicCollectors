using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicCollectors.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddEtchedAndTwoFaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasTwoFaces",
                table: "Cards",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasTwoFaces",
                table: "Cards");
        }
    }
}
