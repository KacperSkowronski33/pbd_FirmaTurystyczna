using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.DTOs.Lookups
{
    public class KrajDto
    {
        public int Id { get; set; }
        public string Nazwa { get; set; } = string.Empty;
    }
    public class CreateKrajDto
    {
        public string Nazwa {  set; get; } = string.Empty;
    }
}
