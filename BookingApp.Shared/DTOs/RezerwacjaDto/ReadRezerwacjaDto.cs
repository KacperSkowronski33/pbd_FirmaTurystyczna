using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.DTOs.RezerwacjaDto
{
    public class ReadRezerwacjaDto
    {
        public int Id { get; set; }
        public string Imie { get; set; } = string.Empty;
        public string Nazwisko { get; set; } = string.Empty;
        public DateTime DataOd { get; set; }
        public DateTime DataDo { get; set; }

        // Zamiast wysyłać całe obiekty, wysyłamy konkretne dane:
        public int HotelId { get; set; }
        public string NazwaHotelu { get; set; } = string.Empty;
    }
}
