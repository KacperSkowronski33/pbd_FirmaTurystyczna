using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Shared.Models;

namespace BookingApp.Shared.DTOs.HotelDto
{
    public class CreateHotelDto
    {
        public string NazwaHotelu {  get; set; } = string.Empty;
        public int LiczbaGwiazdek {  get; set; }
        public int MiejscowoscId { get; set; }
        public Adres Adres { get; set; } = new Adres();
    }
}
