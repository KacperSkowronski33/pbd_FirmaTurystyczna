namespace BookingApp.Shared.DTOs.RaportDto
{
    public class PopularityReportRequestDto
    {
        public DateTime? DataOd { get; set; }
        public DateTime? DataDo { get; set; }

        public bool PokazNazwe { get; set; } = true;
        public bool PokazGwiazdki { get; set; } = true;
        public bool PokazLokalizacje { get; set; } = true;
        public bool PokazLiczbeRezerwacji { get; set; } = true;
        public bool PokazSumeUczestnikow { get; set; } = true;
        public bool PokazPrzychod { get; set; } = true;
        public bool PokazSredniaWartosc { get; set; } = true;
    }
}
