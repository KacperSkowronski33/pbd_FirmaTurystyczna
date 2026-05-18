using BookingApp.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Api.Data
{
    public static class DbSeeder
    {
        public static void Seed(BookingDbContext context)
        {
            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (!context.Role.Any())
            {
                context.Role.AddRange(
                    new Rola { TypRoli = TypRoli.Brak },
                    new Rola { TypRoli = TypRoli.Klient },
                    new Rola { TypRoli = TypRoli.Pracownik },
                    new Rola { TypRoli = TypRoli.Administrator }
                );
                context.SaveChanges();
            }

            if (!context.Pracownicy.Any())
            {
                var adminRola = context.Role.First(r => r.TypRoli == TypRoli.Administrator);
                context.Pracownicy.Add(new Pracownik
                {
                    Imie = "Główny",
                    Nazwisko = "Admin",
                    Email = "admin",
                    Haslo = "123",
                    RolaId = adminRola.Id,
                });
                context.SaveChanges();
            }

            if (!context.Kraje.Any())
            {
                context.Kraje.AddRange(
                    new Kraj { Nazwa = "Polska" },
                    new Kraj { Nazwa = "Hiszpania" },
                    new Kraj { Nazwa = "Egipt" }
                );
                context.SaveChanges();
            }

            if (!context.Miejscowsci.Any())
            {
                var polska = context.Kraje.First(k => k.Nazwa == "Polska");
                var hiszpania = context.Kraje.First(k => k.Nazwa == "Hiszpania");
                var egipt = context.Kraje.First(k => k.Nazwa == "Egipt");

                context.Miejscowsci.AddRange(
                    new Miejscowosc { Nazwa = "Zakopane", KrajId = polska.Id },
                    new Miejscowosc { Nazwa = "Kołobrzeg", KrajId = polska.Id },
                    new Miejscowosc { Nazwa = "Majorka", KrajId = hiszpania.Id },
                    new Miejscowosc { Nazwa = "Hurghada", KrajId = egipt.Id }
                );
                context.SaveChanges();
            }

            if (!context.TypyWyzywienia.Any())
            {
                context.TypyWyzywienia.AddRange(
                    new TypWyzywienia { NazwaWyzywienia = "All Inclusive (All)" },
                    new TypWyzywienia { NazwaWyzywienia = "Śniadania i Obiadokolacje (HB)" },
                    new TypWyzywienia { NazwaWyzywienia = "Tylko Śniadania (BB)" },
                    new TypWyzywienia { NazwaWyzywienia = "Bez Wyżywienia (OV)" }
                );
                context.SaveChanges();
            }

            if (!context.Hotele.Any())
            {
                var zakopane = context.Miejscowsci.First(m => m.Nazwa == "Zakopane");
                var majorka = context.Miejscowsci.First(m => m.Nazwa == "Majorka");
                var hurghada = context.Miejscowsci.First(m => m.Nazwa == "Hurghada");

                var hotel1 = new Hotel
                {
                    NazwaHotelu = "Grand Hotel Giewont View",
                    LiczbaGwiazdek = 5,
                    MiejscowoscId = zakopane.Id,
                    Adres = new Adres { NazwaUlicy = "Krupówki 40", KodPocztowy = "34-500" },
                    ZdjeciaHotelu = new List<ZdjecieHotelu>
                    {
                        new ZdjecieHotelu { UrlZdjecia = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800" },
                        new ZdjecieHotelu { UrlZdjecia = "https://images.unsplash.com/photo-1582719508461-905c673771fd?w=800" }
                    }
                };

                var hotel2 = new Hotel
                {
                    NazwaHotelu = "Viva Mallorca Resort & Spa",
                    LiczbaGwiazdek = 4,
                    MiejscowoscId = majorka.Id,
                    Adres = new Adres { NazwaUlicy = "Av. de la Playa 12", KodPocztowy = "07458" },
                    ZdjeciaHotelu = new List<ZdjecieHotelu>
                    {
                        new ZdjecieHotelu { UrlZdjecia = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800" },
                        new ZdjecieHotelu { UrlZdjecia = "https://images.unsplash.com/photo-1540555700478-4be289fbecef?w=800" }
                    }
                };

                var hotel3 = new Hotel
                {
                    NazwaHotelu = "Desert Rose Aqua Park",
                    LiczbaGwiazdek = 4,
                    MiejscowoscId = hurghada.Id,
                    Adres = new Adres { NazwaUlicy = "Safaga Road KM 16", KodPocztowy = "84511" },
                    ZdjeciaHotelu = new List<ZdjecieHotelu>
                    {
                        new ZdjecieHotelu { UrlZdjecia = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800" }
                    }
                };

                context.Hotele.AddRange(hotel1, hotel2, hotel3);
                context.SaveChanges();
            }

            if (!context.Oferty.Any())
            {
                var hGiewont = context.Hotele.First(h => h.NazwaHotelu == "Grand Hotel Giewont View");
                var hMallorca = context.Hotele.First(h => h.NazwaHotelu == "Viva Mallorca Resort & Spa");
                var hDesert = context.Hotele.First(h => h.NazwaHotelu == "Desert Rose Aqua Park");

                var allInclusive = context.TypyWyzywienia.First(w => w.NazwaWyzywienia.Contains("All"));
                var halfBoard = context.TypyWyzywienia.First(w => w.NazwaWyzywienia.Contains("HB"));
                var breakfastOnly = context.TypyWyzywienia.First(w => w.NazwaWyzywienia.Contains("BB"));

                var oGiewont = new Oferta { HotelId = hGiewont.Id, TypWyzywieniaId = halfBoard.Id };
                var oMallorca = new Oferta { HotelId = hMallorca.Id, TypWyzywieniaId = allInclusive.Id };
                var oDesert = new Oferta { HotelId = hDesert.Id, TypWyzywieniaId = breakfastOnly.Id };

                context.Oferty.AddRange(oGiewont, oMallorca, oDesert);
                context.SaveChanges();

                context.TerminyCeny.AddRange(
                    new TerminCena
                    {
                        OfertaId = oGiewont.Id,
                        DataOd = new DateTime(2026, 07, 01),
                        DataDo = new DateTime(2026, 07, 08, 12, 0, 0, DateTimeKind.Utc),
                        CenaPodstawowa = 1200
                    },
                    new TerminCena
                    {
                        OfertaId = oGiewont.Id,
                        DataOd = new DateTime(2026, 08, 15),
                        DataDo = new DateTime(2026, 08, 22, 12, 0, 0, DateTimeKind.Utc),
                        CenaPodstawowa = 1500
                    },
                    new TerminCena
                    {
                        OfertaId = oMallorca.Id,
                        DataOd = new DateTime(2026, 06, 10),
                        DataDo = new DateTime(2026, 06, 17, 12, 0, 0, DateTimeKind.Utc),
                        CenaPodstawowa = 3450
                    },
                    new TerminCena
                    {
                        OfertaId = oDesert.Id,
                        DataOd = new DateTime(2026, 10, 05),
                        DataDo = new DateTime(2026, 10, 12, 12, 0, 0, DateTimeKind.Utc),
                        CenaPodstawowa = 2250
                    }
                );
                context.SaveChanges();
            }
        }
    }
}