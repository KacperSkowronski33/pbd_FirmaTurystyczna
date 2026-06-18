using System;

namespace BookingApp.Shared.DTOs.RezerwacjaDto
{
    public class ReadRezerwacjaDto
    {
        public int Id { get; set; }
        public string Imie { get; set; } = string.Empty;
        public string Nazwisko { get; set; } = string.Empty;
        public DateTime DataOd { get; set; }
        public DateTime DataDo { get; set; }

        public int HotelId { get; set; }
        public string NazwaHotelu { get; set; } = string.Empty;
        public int LiczbaOsob { get; set; }
        public decimal KwotaCalkowita { get; set; }
        public DateTime DataUtworzenia { get; set; }
        public string StatusPlatnosci { get; set; } = string.Empty;
    }
}