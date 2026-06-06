namespace BookingApp.Shared.DTOs.RaportDto
{
    public class FinancialReportRowDto
    {
        public string Okres { get; set; } = string.Empty;
        public int LiczbaTransakcji { get; set; }
        public int SumaUczestnikow { get; set; }
        public decimal SumaDoplat { get; set; }
        public decimal PrzychodGlowny { get; set; }
        public decimal PrzychodCalkowita { get; set; }
        public decimal SredniaWartosc { get; set; }
    }
}