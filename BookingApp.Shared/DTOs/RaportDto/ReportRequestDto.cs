using BookingApp.Shared.Models;

namespace BookingApp.Shared.DTOs.RaportDto
{
    public class ReportRequestDto
    {
        public DateTime? DataOd { get; set; }
        public DateTime? DataDo { get; set; }
        public Status? StatusRezerwacji { get; set; }

        public bool PokazId { get; set; } = true;
        public bool PokazDataUtworzenia { get; set; } = true;
        public bool PokazKlienta { get; set; } = true;
        public bool PokazEmail { get; set; } = false;
        public bool PokazHotel { get; set; } = true;
        public bool PokazMiejscowosc { get; set; } = true;
        public bool PokazLiczbaOsob { get; set; } = true;
        public bool PokazKwote { get; set; } = true;
    }
}