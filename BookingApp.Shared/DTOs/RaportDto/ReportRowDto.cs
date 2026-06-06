namespace BookingApp.Shared.DTOs.RaportDto
{
    public class ReportRowDto
    {
        public int Id { get; set; }
        public DateTime DataUtworzenia { get; set; }
        public string KlientNazwa { get; set; } = string.Empty;
        public string KlientEmail { get; set; } = string.Empty;
        public string HotelNazwa { get; set; } = string.Empty;
        public string MiejscowoscNazwa { get; set; } = string.Empty;
        public int LiczbaOsob { get; set; }
        public decimal KwotaCalkowita { get; set; }
    }
}