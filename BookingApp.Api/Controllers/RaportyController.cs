using BookingApp.Api.Data;
using BookingApp.Shared.ApiResponse;
using BookingApp.Shared.DTOs.RaportDto;
using BookingApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BookingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Pracownik,Administrator")]
    public class RaportyController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public RaportyController(BookingDbContext context)
        {
            _context = context;
        }

        #region MODUŁ 1: RAPORT SPRZEDAŻY
        private IQueryable<ReportRowDto> PobierzDaneRaportuQuery(ReportRequestDto dto)
        {
            var query = _context.Rezerwacje
                .Include(r => r.Klient)
                .Include(r => r.Statusy)
                .Include(r => r.TerminCena)
                    .ThenInclude(t => t.Oferta)
                        .ThenInclude(o => o.Hotel)
                            .ThenInclude(h => h.Miejscowosc)
                .AsNoTracking();

            if (dto.DataOd.HasValue) query = query.Where(r => r.DataUtworzenia >= dto.DataOd.Value);
            if (dto.DataDo.HasValue) query = query.Where(r => r.DataUtworzenia <= dto.DataDo.Value);

            if (dto.StatusRezerwacji.HasValue)
            {
                query = query.Where(r => r.Statusy
                    .OrderByDescending(s => s.DataZmiany)
                    .Select(s => s.StanStatusu)
                    .FirstOrDefault() == dto.StatusRezerwacji.Value);
            }

            return query.OrderByDescending(r => r.DataUtworzenia).Select(r => new ReportRowDto
            {
                Id = r.Id,
                DataUtworzenia = r.DataUtworzenia,
                KlientNazwa = r.Klient.Imie + " " + r.Klient.Nazwisko,
                KlientEmail = r.Klient.Email,
                HotelNazwa = r.TerminCena.Oferta.Hotel.NazwaHotelu,
                MiejscowoscNazwa = r.TerminCena.Oferta.Hotel.Miejscowosc != null ? r.TerminCena.Oferta.Hotel.Miejscowosc.Nazwa : "Brak",
                LiczbaOsob = r.LiczbaOsob,
                KwotaCalkowita = r.KwotaCalkowita
            });
        }

        [HttpPost("preview")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReportRowDto>>>> PobierzPodglad(ReportRequestDto dto)
        {
            var dane = await PobierzDaneRaportuQuery(dto).Take(10).ToListAsync();
            return Ok(ApiResponse<IEnumerable<ReportRowDto>>.Ok(dane));
        }

        [HttpPost("pdf")]
        public async Task<IActionResult> GenerujPdf(ReportRequestDto dto)
        {
            var dane = await PobierzDaneRaportuQuery(dto).ToListAsync();
            var dokument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("WYMARZONE WAKACJE - RAPORT SPRZEDAŻY").FontSize(16).Bold().FontColor(Colors.Blue.Darken3);
                            col.Item().Text($"Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(9).Italic();
                        });
                    });

                    page.Content().PaddingTop(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            if (dto.PokazId) columns.ConstantColumn(40);
                            if (dto.PokazDataUtworzenia) columns.RelativeColumn(2);
                            if (dto.PokazKlienta) columns.RelativeColumn(3);
                            if (dto.PokazEmail) columns.RelativeColumn(3);
                            if (dto.PokazHotel) columns.RelativeColumn(4);
                            if (dto.PokazMiejscowosc) columns.RelativeColumn(3);
                            if (dto.PokazLiczbaOsob) columns.ConstantColumn(40);
                            if (dto.PokazKwote) columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            static void StylNaglowka(IContainer c, string tekst) =>
                                c.Background(Colors.Blue.Darken2).Padding(5).AlignCenter().AlignMiddle().Text(tekst).Bold().FontColor(Colors.White).FontSize(9);

                            if (dto.PokazId) StylNaglowka(header.Cell(), "ID");
                            if (dto.PokazDataUtworzenia) StylNaglowka(header.Cell(), "Data");
                            if (dto.PokazKlienta) StylNaglowka(header.Cell(), "Klient");
                            if (dto.PokazEmail) StylNaglowka(header.Cell(), "Email");
                            if (dto.PokazHotel) StylNaglowka(header.Cell(), "Hotel");
                            if (dto.PokazMiejscowosc) StylNaglowka(header.Cell(), "Miejscowość");
                            if (dto.PokazLiczbaOsob) StylNaglowka(header.Cell(), "Os.");
                            if (dto.PokazKwote) StylNaglowka(header.Cell(), "Kwota");
                        });

                        foreach (var item in dane)
                        {
                            static void StylKomorki(IContainer c, string tekst, bool alignRight = false)
                            {
                                var cell = c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignMiddle();
                                if (alignRight) cell.AlignRight().Text(tekst);
                                else cell.Text(tekst);
                            }

                            if (dto.PokazId) StylKomorki(table.Cell(), item.Id.ToString());
                            if (dto.PokazDataUtworzenia) StylKomorki(table.Cell(), item.DataUtworzenia.ToString("dd.MM.yyyy"));
                            if (dto.PokazKlienta) StylKomorki(table.Cell(), item.KlientNazwa);
                            if (dto.PokazEmail) StylKomorki(table.Cell(), item.KlientEmail);
                            if (dto.PokazHotel) StylKomorki(table.Cell(), item.HotelNazwa);
                            if (dto.PokazMiejscowosc) StylKomorki(table.Cell(), item.MiejscowoscNazwa);
                            if (dto.PokazLiczbaOsob) StylKomorki(table.Cell(), item.LiczbaOsob.ToString(), alignRight: true);
                            if (dto.PokazKwote) StylKomorki(table.Cell(), item.KwotaCalkowita.ToString("C2"), alignRight: true);
                        }
                    });

                    page.Footer().AlignRight().Text(x =>
                    {
                        x.Span("Strona "); x.CurrentPageNumber(); x.Span(" z "); x.TotalPages();
                    });
                });
            });

            var stream = new MemoryStream();
            dokument.GeneratePdf(stream);
            stream.Position = 0;
            return File(stream, "application/pdf", $"Raport_Sprzedazy_{DateTime.Now:yyyyMMdd}.pdf");
        }
        #endregion

        #region MODUŁ 2: RAPORT POPULARNOŚCI OBIEKTÓW
        private IQueryable<PopularityReportRowDto> PobierzPopularnoscObiektowQuery(PopularityReportRequestDto dto)
        {
            var query = _context.Rezerwacje
                .Where(r => (!dto.DataOd.HasValue || r.DataUtworzenia >= dto.DataOd.Value) &&
                            (!dto.DataDo.HasValue || r.DataUtworzenia <= dto.DataDo.Value))
                .GroupBy(r => r.TerminCena.Oferta.Hotel)
                .Select(g => new PopularityReportRowDto
                {
                    HotelNazwa = g.Key.NazwaHotelu,
                    LiczbaGwiazdek = g.Key.LiczbaGwiazdek,
                    Lokalizacja = g.Key.Miejscowosc != null ? g.Key.Miejscowosc.Nazwa : "Brak danych",
                    LiczbaRezerwacji = g.Count(),
                    SumaUczestnikow = g.Sum(r => r.LiczbaOsob),
                    GenerowanyPrzychod = g.Sum(r => r.KwotaCalkowita),
                    SredniaWartosc = g.Count() > 0 ? g.Sum(r => r.KwotaCalkowita) / g.Count() : 0
                })
                .OrderByDescending(r => r.LiczbaRezerwacji);

            return query;
        }

        [HttpPost("popularity/preview")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PopularityReportRowDto>>>> PobierzPopularnoscPodglad(PopularityReportRequestDto dto)
        {
            var dane = await PobierzPopularnoscObiektowQuery(dto).Take(10).ToListAsync();
            return Ok(ApiResponse<IEnumerable<PopularityReportRowDto>>.Ok(dane));
        }

        [HttpPost("popularity/pdf")]
        public async Task<IActionResult> GenerujPopularnoscPdf(PopularityReportRequestDto dto)
        {
            var dane = await PobierzPopularnoscObiektowQuery(dto).ToListAsync();
            var dokument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("WYMARZONE WAKACJE - RANKING POPULARNOŚCI HOTELI").FontSize(16).Bold().FontColor(Colors.Orange.Darken3);
                            col.Item().Text($"Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(9).Italic();
                        });
                    });

                    page.Content().PaddingTop(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            if (dto.PokazNazwe) columns.RelativeColumn(4);
                            if (dto.PokazGwiazdki) columns.ConstantColumn(50);
                            if (dto.PokazLokalizacje) columns.RelativeColumn(3);
                            if (dto.PokazLiczbeRezerwacji) columns.RelativeColumn(2);
                            if (dto.PokazSumeUczestnikow) columns.RelativeColumn(2);
                            if (dto.PokazPrzychod) columns.RelativeColumn(3);
                            if (dto.PokazSredniaWartosc) columns.RelativeColumn(3);
                        });

                        table.Header(header =>
                        {
                            static void StylNaglowka(IContainer c, string tekst) =>
                                c.Background(Colors.Orange.Darken2).Padding(5).AlignCenter().AlignMiddle().Text(tekst).Bold().FontColor(Colors.White).FontSize(9);

                            if (dto.PokazNazwe) StylNaglowka(header.Cell(), "Nazwa Hotelu");
                            if (dto.PokazGwiazdki) StylNaglowka(header.Cell(), "Gwiazdki");
                            if (dto.PokazLokalizacje) StylNaglowka(header.Cell(), "Miejscowość");
                            if (dto.PokazLiczbeRezerwacji) StylNaglowka(header.Cell(), "Liczba Rezerwacji");
                            if (dto.PokazSumeUczestnikow) StylNaglowka(header.Cell(), "Suma Osób");
                            if (dto.PokazPrzychod) StylNaglowka(header.Cell(), "Przychód łączny");
                            if (dto.PokazSredniaWartosc) StylNaglowka(header.Cell(), "Śr. Wartość");
                        });

                        foreach (var item in dane)
                        {
                            static void StylKomorki(IContainer c, string tekst, bool alignRight = false)
                            {
                                var cell = c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignMiddle();
                                if (alignRight) cell.AlignRight().Text(tekst);
                                else cell.Text(tekst);
                            }

                            if (dto.PokazNazwe) StylKomorki(table.Cell(), item.HotelNazwa);
                            if (dto.PokazGwiazdki) StylKomorki(table.Cell(), $"{item.LiczbaGwiazdek}*");
                            if (dto.PokazLokalizacje) StylKomorki(table.Cell(), item.Lokalizacja);
                            if (dto.PokazLiczbeRezerwacji) StylKomorki(table.Cell(), item.LiczbaRezerwacji.ToString(), alignRight: true);
                            if (dto.PokazSumeUczestnikow) StylKomorki(table.Cell(), item.SumaUczestnikow.ToString(), alignRight: true);
                            if (dto.PokazPrzychod) StylKomorki(table.Cell(), item.GenerowanyPrzychod.ToString("C2"), alignRight: true);
                            if (dto.PokazSredniaWartosc) StylKomorki(table.Cell(), item.SredniaWartosc.ToString("C2"), alignRight: true);
                        }
                    });

                    page.Footer().AlignRight().Text(x =>
                    {
                        x.Span("Strona "); x.CurrentPageNumber(); x.Span(" z "); x.TotalPages();
                    });
                });
            });

            var stream = new MemoryStream();
            dokument.GeneratePdf(stream);
            stream.Position = 0;
            return File(stream, "application/pdf", $"Raport_Popularnosci_{DateTime.Now:yyyyMMdd}.pdf");
        }
        #endregion

        #region MODUŁ 3: RAPORT FINANSOWY I ZYSKÓW
        private async Task<List<FinancialReportRowDto>> GenerujDaneFinansowe(FinancialReportRequestDto dto)
        {
            var rezerwacje = await _context.Rezerwacje
                .Include(r => r.Doplaty)
                .Where(r => (!dto.DataOd.HasValue || r.DataUtworzenia >= dto.DataOd.Value) &&
                            (!dto.DataDo.HasValue || r.DataUtworzenia <= dto.DataDo.Value))
                .AsNoTracking()
                .ToListAsync();

            var zgrupowane = rezerwacje.GroupBy(r =>
            {
                if (dto.Grupowanie == "dzien") return r.DataUtworzenia.ToString("dd.MM.yyyy");
                if (dto.Grupowanie == "kwartal") return $"{r.DataUtworzenia.Year} - Q{(r.DataUtworzenia.Month - 1) / 3 + 1}";
                return r.DataUtworzenia.ToString("MM.yyyy"); // Domyślnie miesięcznie
            })
            .Select(g =>
            {
                decimal sumaDoplat = g.Sum(r => r.Doplaty.Sum(d => d.KwotaDoplaty));
                decimal przychodGlowny = g.Sum(r => r.KwotaCalkowita);
                int transakcje = g.Count();

                return new FinancialReportRowDto
                {
                    Okres = g.Key,
                    LiczbaTransakcji = transakcje,
                    SumaUczestnikow = g.Sum(r => r.LiczbaOsob),
                    SumaDoplat = sumaDoplat,
                    PrzychodGlowny = przychodGlowny,
                    PrzychodCalkowita = przychodGlowny + sumaDoplat,
                    SredniaWartosc = transakcje > 0 ? (przychodGlowny + sumaDoplat) / transakcje : 0
                };
            }).ToList();

            return zgrupowane;
        }

        [HttpPost("financial/preview")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FinancialReportRowDto>>>> PobierzFinansePodglad(FinancialReportRequestDto dto)
        {
            var dane = await GenerujDaneFinansowe(dto);
            return Ok(ApiResponse<IEnumerable<FinancialReportRowDto>>.Ok(dane.Take(10)));
        }

        [HttpPost("financial/pdf")]
        public async Task<IActionResult> GenerujFinansePdf(FinancialReportRequestDto dto)
        {
            var dane = await GenerujDaneFinansowe(dto);

            var dokument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("WYMARZONE WAKACJE - ANALITYCZNY RAPORT FINANSOWY").FontSize(16).Bold().FontColor(Colors.Green.Darken3);
                            col.Item().Text($"Grupowanie: {dto.Grupowanie.ToUpper()} | Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(9).Italic();
                        });
                    });

                    page.Content().PaddingTop(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            if (dto.PokazOkres) columns.RelativeColumn(3);
                            if (dto.PokazLiczbeTransakcji) columns.RelativeColumn(2);
                            if (dto.PokazSumeUczestnikow) columns.RelativeColumn(2);
                            if (dto.PokazSumeDoplat) columns.RelativeColumn(3);
                            if (dto.PokazPrzychodGlowny) columns.RelativeColumn(3);
                            if (dto.PokazPrzychodCalkowity) columns.RelativeColumn(3);
                            if (dto.PokazSredniaWartosc) columns.RelativeColumn(3);
                        });

                        table.Header(header =>
                        {
                            static void StylNaglowka(IContainer c, string tekst) =>
                                c.Background(Colors.Green.Darken2).Padding(5).AlignCenter().AlignMiddle().Text(tekst).Bold().FontColor(Colors.White).FontSize(9);

                            if (dto.PokazOkres) StylNaglowka(header.Cell(), "Okres rozliczeniowy");
                            if (dto.PokazLiczbeTransakcji) StylNaglowka(header.Cell(), "Liczba transakcji");
                            if (dto.PokazSumeUczestnikow) StylNaglowka(header.Cell(), "Łącznie gości");
                            if (dto.PokazSumeDoplat) StylNaglowka(header.Cell(), "Suma dopłat");
                            if (dto.PokazPrzychodGlowny) StylNaglowka(header.Cell(), "Przychód główny");
                            if (dto.PokazPrzychodCalkowity) StylNaglowka(header.Cell(), "Przychód łączny");
                            if (dto.PokazSredniaWartosc) StylNaglowka(header.Cell(), "Śr. Koszyk");
                        });

                        int tTransakcje = 0; int tGoscie = 0; decimal tDoplaty = 0; decimal tGlowny = 0; decimal tCalkowity = 0;

                        foreach (var item in dane)
                        {
                            tTransakcje += item.LiczbaTransakcji;
                            tGoscie += item.SumaUczestnikow;
                            tDoplaty += item.SumaDoplat;
                            tGlowny += item.PrzychodGlowny;
                            tCalkowity += item.PrzychodCalkowita;

                            static void StylKomorki(IContainer c, string tekst, bool alignRight = false)
                            {
                                var cell = c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignMiddle();
                                if (alignRight) cell.AlignRight().Text(tekst);
                                else cell.Text(tekst);
                            }

                            if (dto.PokazOkres) StylKomorki(table.Cell(), item.Okres);
                            if (dto.PokazLiczbeTransakcji) StylKomorki(table.Cell(), item.LiczbaTransakcji.ToString(), alignRight: true);
                            if (dto.PokazSumeUczestnikow) StylKomorki(table.Cell(), item.SumaUczestnikow.ToString(), alignRight: true);
                            if (dto.PokazSumeDoplat) StylKomorki(table.Cell(), item.SumaDoplat.ToString("C2"), alignRight: true);
                            if (dto.PokazPrzychodGlowny) StylKomorki(table.Cell(), item.PrzychodGlowny.ToString("C2"), alignRight: true);
                            if (dto.PokazPrzychodCalkowity) StylKomorki(table.Cell(), item.PrzychodCalkowita.ToString("C2"), alignRight: true);
                            if (dto.PokazSredniaWartosc) StylKomorki(table.Cell(), item.SredniaWartosc.ToString("C2"), alignRight: true);
                        }

                        static void StylPodsumowania(IContainer c, string tekst, bool alignRight = false)
                        {
                            var cell = c.Background(Colors.Grey.Lighten3).Padding(5).AlignMiddle();
                            if (alignRight) cell.AlignRight().Text(tekst).Bold().FontColor(Colors.Green.Darken4);
                            else cell.Text(tekst).Bold();
                        }

                        if (dto.PokazOkres) StylPodsumowania(table.Cell(), "SUMA CAŁKOWITA:");
                        if (dto.PokazLiczbeTransakcji) StylPodsumowania(table.Cell(), tTransakcje.ToString(), alignRight: true);
                        if (dto.PokazSumeUczestnikow) StylPodsumowania(table.Cell(), tGoscie.ToString(), alignRight: true);
                        if (dto.PokazSumeDoplat) StylPodsumowania(table.Cell(), tDoplaty.ToString("C2"), alignRight: true);
                        if (dto.PokazPrzychodGlowny) StylPodsumowania(table.Cell(), tGlowny.ToString("C2"), alignRight: true);
                        if (dto.PokazPrzychodCalkowity) StylPodsumowania(table.Cell(), tCalkowity.ToString("C2"), alignRight: true);
                        if (dto.PokazSredniaWartosc) StylPodsumowania(table.Cell(), (tTransakcje > 0 ? tCalkowity / tTransakcje : 0).ToString("C2"), alignRight: true);
                    });

                    page.Footer().AlignRight().Text(x =>
                    {
                        x.Span("Strona "); x.CurrentPageNumber(); x.Span(" z "); x.TotalPages();
                    });
                });
            });

            var stream = new MemoryStream();
            dokument.GeneratePdf(stream);
            stream.Position = 0;
            return File(stream, "application/pdf", $"Raport_Finansowy_{DateTime.Now:yyyyMMdd}.pdf");
        }
        #endregion
    }
}