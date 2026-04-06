using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    public class Adres
    {
        public string NazwaUlicy { get; set; } = string.Empty;
        public string NumerBudynku { get; set; } = string.Empty;
        public string? NumerLokalu { get; set; }
        public string KodPocztowy { get; set; } = string.Empty;
    }
}
