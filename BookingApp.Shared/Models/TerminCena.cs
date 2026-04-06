using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    public class TerminCena
    {
        public int Id { get; set; } 
        public int OfertaId { get; set; }
        public DateOnly DataOd {  get; set; }
        public DateTime DataDo { get; set; }
        public decimal CenaPodstawowa { get; set; }
        public Oferta Oferta {  get; set; }
        public ICollection<Rezerwacja> Rezerwacje { get; set; } = new List<Rezerwacja>();
    }
}
