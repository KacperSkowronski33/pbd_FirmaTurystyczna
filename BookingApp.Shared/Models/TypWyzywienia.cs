using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    public class TypWyzywienia
    {
        public int Id { get; set; }
        public string NazwaWyzywienia { get; set; }
        public ICollection<Oferta> Oferty { get; set; } = new List<Oferta>();
    }
}
