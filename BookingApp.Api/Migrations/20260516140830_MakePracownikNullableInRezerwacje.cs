using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakePracownikNullableInRezerwacje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezerwacje_Pracownicy_PracownikId",
                table: "Rezerwacje");

            migrationBuilder.AlterColumn<int>(
                name: "PracownikId",
                table: "Rezerwacje",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Rezerwacje_Pracownicy_PracownikId",
                table: "Rezerwacje",
                column: "PracownikId",
                principalTable: "Pracownicy",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezerwacje_Pracownicy_PracownikId",
                table: "Rezerwacje");

            migrationBuilder.AlterColumn<int>(
                name: "PracownikId",
                table: "Rezerwacje",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rezerwacje_Pracownicy_PracownikId",
                table: "Rezerwacje",
                column: "PracownikId",
                principalTable: "Pracownicy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
