using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string NazwaHotelu { get; set; } = string.Empty;
        private int _liczbaGwiazdek;
        public int LiczbaGwiazdek {  get => _liczbaGwiazdek; set { 
                if (value > 5) _liczbaGwiazdek = 5; 
                else if (value < 0) _liczbaGwiazdek = 0; 
                else _liczbaGwiazdek = value; } 
        }
        public int MiejscowoscId { get; set; }
        public Adres Adres { get; set; } = new Adres();  //detale adresow
        public Miejscowosc? Miejscowosc { get; set; } //miejscowosc jako osobne pole, ze wzgledu na relacje w bazie
        public ICollection<RodzajPokoju> RodzajePokojow { get; set; } = new List<RodzajPokoju>();
        public ICollection<ZdjecieHotelu> ZdjeciaHotelu { get; set; } = new List<ZdjecieHotelu>();
        public ICollection<Oferta> Oferty { get; set; } = new List<Oferta>();

    }
}
