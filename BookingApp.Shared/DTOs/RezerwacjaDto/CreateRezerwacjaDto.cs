using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Shared.Models;

namespace BookingApp.Shared.DTOs.RezerwacjaDto
{
    public class CreateRezerwacjaDto
    {
        public int HotelId { get; set; }
        public int KlientId { get; set; }
        public int PracownikId { get; set; }
        public int TerminCenaId { get; set; } 
        public int LiczbaOsob {  get; set; }
    }
}
