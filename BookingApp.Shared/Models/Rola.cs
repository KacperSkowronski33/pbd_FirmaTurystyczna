using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    public enum TypRoli
    {
        Brak,
        Pracownik,
        Administrator
    }
    public class Rola
    {
        public int Id { get; set; }
        public TypRoli TypRoli { get; set; }
        public ICollection<Pracownik> Pracownicy { get; set; } = new List<Pracownik>();
    }
}
