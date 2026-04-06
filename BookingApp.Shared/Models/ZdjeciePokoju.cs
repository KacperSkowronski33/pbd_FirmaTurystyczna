using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    public class ZdjeciePokoju
    {
        public int Id { get; set; }
        public int RodzajPokojuId { get; set; }
        public string UrlZdjecia { get; set; } = string.Empty;

        public RodzajPokoju RodzajPokoju { get; set; } = null!;
    }
}
