using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Shared.Models;

namespace BookingApp.Shared.DTOs.HotelDto
{
    public class ReadHotelDto
    {
        public int Id { get; set; }
        public string NazwaHotelu { get; set; } = string.Empty;
        public int LiczbaGwiazdek { get; set; }
        public Adres Adres { get; set; } = new Adres();
        public string NazwaMiejscowosci { get; set; } = string.Empty;

    }
}
