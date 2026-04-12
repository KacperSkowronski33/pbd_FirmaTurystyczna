using BookingApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.DTOs.KlientDto
{
    public class ReadKlientDto
    {
        public int Id { get; set; }
        public string Imie { get; set; } = string.Empty;
        public string Nazwisko { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Haslo { get; set; } = string.Empty;
        public string? AdresKlienta { get; set; }

        public ICollection<Rezerwacja> Rezerwacje = new List<Rezerwacja>();
    }
}
