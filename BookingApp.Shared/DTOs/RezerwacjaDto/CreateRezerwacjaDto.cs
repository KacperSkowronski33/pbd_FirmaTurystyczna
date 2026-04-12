using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.DTOs.RezerwacjaDto
{
    public class CreateRezerwacjaDto
    {
        public int KlientId { get; set; }
        public int TerminCenaId { get; set; }
        public int PracownikId { get; set; }
        public int LiczbaOsob { get; set; }
        public decimal KwotaCalkowita { get; set; }
        public DateTime DataUtworzenia { get; set; }
    }
}