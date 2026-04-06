using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.Models
{
    
    public class Miejscowosc
    {
        public int Id { get; set; }
        public int KrajId { get; set; }
        public string Nazwa {  get; set; } = string.Empty;
        public ICollection<Hotel> Hotele { get; set; } = new List<Hotel>();

    }
}
