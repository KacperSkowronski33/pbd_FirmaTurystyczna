using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    public class Rezerwacja
    {
        public int Id { get; set; } 
        public int KlientId { get; set; }
        public int TerminCenaId { get; set; }
        public int PracownikId { get; set; }
        public int LiczbaOsob {  get; set; }
        public decimal KwotaCalkowita { get; set; }
        public DateTime DataUtworzenia {  get; set; }

        public Klient Klient { get; set; }
        public TerminCena TerminCena { get; set; }
        public Pracownik Pracownik { get; set; }

        public ICollection<StatusRezerwacji> Statusy { get; set; } = new List<StatusRezerwacji>();
        public ICollection<Doplata> Doplaty { get; set; } = new List<Doplata>();
    }
}
