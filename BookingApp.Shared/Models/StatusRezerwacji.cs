using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    public enum Status
    {
        Oczekiwanie_Na_Zatwierdzenie,
        Zatwierdzono,
        Anulowano,
        Zakonczono
    }
    public class StatusRezerwacji
    {
        public int Id { get; set; }
        public int RezerwacjaId { get; set; }
        public Status StanStatusu { get; set; }
        public DateTime DataZmiany { get; set; }

        public Rezerwacja Rezerwacja { get; set; }
    }
}
