using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    public class ZdjecieHotelu
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string UrlZdjecia { get; set; } = string.Empty;

        public Hotel Hotel { get; set; } = null!;
    }
}
