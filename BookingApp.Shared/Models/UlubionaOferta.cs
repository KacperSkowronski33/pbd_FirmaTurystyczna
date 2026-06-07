using System;

namespace BookingApp.Shared.Models
{
    public class UlubionaOferta
    {
        public int Id { get; set; }
        public int KlientId { get; set; }
        public int OfertaId { get; set; }
        public Klient Klient { get; set; } = null!;
        public Oferta Oferta { get; set; } = null!;
    }
}