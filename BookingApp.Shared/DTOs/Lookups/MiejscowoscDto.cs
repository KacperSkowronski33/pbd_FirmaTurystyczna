using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.DTOs.Lookups
{
    public class MiejscowoscDto
    {
        public int Id { get; set; }
        public string Nazwa { get; set; } = string.Empty;
        public int KrajId { get; set; }
    }

    public class CreateMiejscowoscDto
    {
        public string Nazwa {  set; get; } = string.Empty;
        public int KrajId { get; set; }
    }
}
