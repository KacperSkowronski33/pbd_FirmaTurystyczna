using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    public class RodzajPokoju
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string Nazwa { get; set; } = string.Empty;
        public int MaxLiczbaOsob {  get; set; }
        public Hotel Hotel { get; set; } = null!;
        public ICollection<ZdjeciePokoju> Zdjecia = new List<ZdjeciePokoju>();

    }
}
