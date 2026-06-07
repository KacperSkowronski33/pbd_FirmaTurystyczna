namespace BookingApp.Shared.DTOs.RaportDto
{
    public class FinancialReportRequestDto
    {
        public DateTime? DataOd { get; set; }
        public DateTime? DataDo { get; set; }

        public string Grupowanie { get; set; } = "miesiac";

        public bool PokazOkres { get; set; } = true;
        public bool PokazLiczbeTransakcji { get; set; } = true;
        public bool PokazSumeUczestnikow { get; set; } = true;
        public bool PokazSumeDoplat { get; set; } = true;
        public bool PokazPrzychodGlowny { get; set; } = true;
        public bool PokazPrzychodCalkowity { get; set; } = true;
        public bool PokazSredniaWartosc { get; set; } = true;
    }
}