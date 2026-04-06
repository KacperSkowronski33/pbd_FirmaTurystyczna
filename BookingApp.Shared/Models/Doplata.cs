using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    public class Doplata
    {
        public int Id { get; set; }
        public int RezerwacjaId { get; set; }
        public string NazwaDoplaty { get; set; }
        public decimal KwotaDoplaty { get; set; }
        public Rezerwacja Rezerwacja { get; set; }
    }
}
