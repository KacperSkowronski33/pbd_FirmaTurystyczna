using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    public class Kraj
    {
        public int Id {  get; set; }
        public string Nazwa {  get; set; } = string.Empty;
        public ICollection<Miejscowosc> Miejscowosci { get; set; } = new List<Miejscowosc>();
    }
}
