using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class DodajPlatnosciPayU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PayUOrderId",
                table: "Rezerwacje",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusPlatnosci",
                table: "Rezerwacje",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayUOrderId",
                table: "Rezerwacje");

            migrationBuilder.DropColumn(
                name: "StatusPlatnosci",
                table: "Rezerwacje");
        }
    }
}
