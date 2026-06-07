namespace BookingApp.Shared.DTOs.RaportDto
{
    public class PopularityReportRowDto
    {
        public string HotelNazwa { get; set; } = string.Empty;
        public int LiczbaGwiazdek { get; set; }
        public string Lokalizacja { get; set; } = string.Empty;
        public int LiczbaRezerwacji { get; set; }
        public int SumaUczestnikow { get; set; }
        public decimal GenerowanyPrzychod { get; set; }
        public decimal SredniaWartosc { get; set; }
    }
}