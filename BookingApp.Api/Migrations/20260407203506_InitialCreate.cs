using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookingApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Klienci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Imie = table.Column<string>(type: "text", nullable: false),
                    Nazwisko = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Haslo = table.Column<string>(type: "text", nullable: false),
                    AdresKlienta = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klienci", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kraje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nazwa = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kraje", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypRoli = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypyWyzywienia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NazwaWyzywienia = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypyWyzywienia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Miejscowsci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KrajId = table.Column<int>(type: "integer", nullable: false),
                    Nazwa = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Miejscowsci", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Miejscowsci_Kraje_KrajId",
                        column: x => x.KrajId,
                        principalTable: "Kraje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pracownicy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RolaId = table.Column<int>(type: "integer", nullable: false),
                    Imie = table.Column<string>(type: "text", nullable: false),
                    Nazwisko = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Haslo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pracownicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pracownicy_Role_RolaId",
                        column: x => x.RolaId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hotele",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NazwaHotelu = table.Column<string>(type: "text", nullable: false),
                    LiczbaGwiazdek = table.Column<int>(type: "integer", nullable: false),
                    Adres_NazwaUlicy = table.Column<string>(type: "text", nullable: false),
                    Adres_NumerBudynku = table.Column<string>(type: "text", nullable: false),
                    Adres_NumerLokalu = table.Column<string>(type: "text", nullable: true),
                    Adres_KodPocztowy = table.Column<string>(type: "text", nullable: false),
                    MiejscowoscId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotele", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotele_Miejscowsci_MiejscowoscId",
                        column: x => x.MiejscowoscId,
                        principalTable: "Miejscowsci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Oferty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HotelId = table.Column<int>(type: "integer", nullable: false),
                    TypWyzywieniaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oferty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Oferty_Hotele_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Oferty_TypyWyzywienia_TypWyzywieniaId",
                        column: x => x.TypWyzywieniaId,
                        principalTable: "TypyWyzywienia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RodzajePokojow",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HotelId = table.Column<int>(type: "integer", nullable: false),
                    Nazwa = table.Column<string>(type: "text", nullable: false),
                    MaxLiczbaOsob = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RodzajePokojow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RodzajePokojow_Hotele_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZdjeciaHotelow",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HotelId = table.Column<int>(type: "integer", nullable: false),
                    UrlZdjecia = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZdjeciaHotelow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZdjeciaHotelow_Hotele_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TerminyCeny",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfertaId = table.Column<int>(type: "integer", nullable: false),
                    DataOd = table.Column<DateOnly>(type: "date", nullable: false),
                    DataDo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CenaPodstawowa = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminyCeny", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TerminyCeny_Oferty_OfertaId",
                        column: x => x.OfertaId,
                        principalTable: "Oferty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZdjeciaPokojow",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RodzajPokojuId = table.Column<int>(type: "integer", nullable: false),
                    UrlZdjecia = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZdjeciaPokojow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZdjeciaPokojow_RodzajePokojow_RodzajPokojuId",
                        column: x => x.RodzajPokojuId,
                        principalTable: "RodzajePokojow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rezerwacje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KlientId = table.Column<int>(type: "integer", nullable: false),
                    TerminCenaId = table.Column<int>(type: "integer", nullable: false),
                    PracownikId = table.Column<int>(type: "integer", nullable: false),
                    LiczbaOsob = table.Column<int>(type: "integer", nullable: false),
                    KwotaCalkowita = table.Column<decimal>(type: "numeric", nullable: false),
                    DataUtworzenia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezerwacje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rezerwacje_Klienci_KlientId",
                        column: x => x.KlientId,
                        principalTable: "Klienci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rezerwacje_Pracownicy_PracownikId",
                        column: x => x.PracownikId,
                        principalTable: "Pracownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rezerwacje_TerminyCeny_TerminCenaId",
                        column: x => x.TerminCenaId,
                        principalTable: "TerminyCeny",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Doplaty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RezerwacjaId = table.Column<int>(type: "integer", nullable: false),
                    NazwaDoplaty = table.Column<string>(type: "text", nullable: false),
                    KwotaDoplaty = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doplaty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doplaty_Rezerwacje_RezerwacjaId",
                        column: x => x.RezerwacjaId,
                        principalTable: "Rezerwacje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatusyRezerwacji",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RezerwacjaId = table.Column<int>(type: "integer", nullable: false),
                    StanStatusu = table.Column<int>(type: "integer", nullable: false),
                    DataZmiany = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusyRezerwacji", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusyRezerwacji_Rezerwacje_RezerwacjaId",
                        column: x => x.RezerwacjaId,
                        principalTable: "Rezerwacje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Doplaty_RezerwacjaId",
                table: "Doplaty",
                column: "RezerwacjaId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotele_MiejscowoscId",
                table: "Hotele",
                column: "MiejscowoscId");

            migrationBuilder.CreateIndex(
                name: "IX_Miejscowsci_KrajId",
                table: "Miejscowsci",
                column: "KrajId");

            migrationBuilder.CreateIndex(
                name: "IX_Oferty_HotelId",
                table: "Oferty",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Oferty_TypWyzywieniaId",
                table: "Oferty",
                column: "TypWyzywieniaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pracownicy_RolaId",
                table: "Pracownicy",
                column: "RolaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezerwacje_KlientId",
                table: "Rezerwacje",
                column: "KlientId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezerwacje_PracownikId",
                table: "Rezerwacje",
                column: "PracownikId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezerwacje_TerminCenaId",
                table: "Rezerwacje",
                column: "TerminCenaId");

            migrationBuilder.CreateIndex(
                name: "IX_RodzajePokojow_HotelId",
                table: "RodzajePokojow",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusyRezerwacji_RezerwacjaId",
                table: "StatusyRezerwacji",
                column: "RezerwacjaId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminyCeny_OfertaId",
                table: "TerminyCeny",
                column: "OfertaId");

            migrationBuilder.CreateIndex(
                name: "IX_ZdjeciaHotelow_HotelId",
                table: "ZdjeciaHotelow",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_ZdjeciaPokojow_RodzajPokojuId",
                table: "ZdjeciaPokojow",
                column: "RodzajPokojuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Doplaty");

            migrationBuilder.DropTable(
                name: "StatusyRezerwacji");

            migrationBuilder.DropTable(
                name: "ZdjeciaHotelow");

            migrationBuilder.DropTable(
                name: "ZdjeciaPokojow");

            migrationBuilder.DropTable(
                name: "Rezerwacje");

            migrationBuilder.DropTable(
                name: "RodzajePokojow");

            migrationBuilder.DropTable(
                name: "Klienci");

            migrationBuilder.DropTable(
                name: "Pracownicy");

            migrationBuilder.DropTable(
                name: "TerminyCeny");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Oferty");

            migrationBuilder.DropTable(
                name: "Hotele");

            migrationBuilder.DropTable(
                name: "TypyWyzywienia");

            migrationBuilder.DropTable(
                name: "Miejscowsci");

            migrationBuilder.DropTable(
                name: "Kraje");
        }
    }
}
