using BookingApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.DTOs.OfertaDto
{
    public class CreateOfertaDto
    {
        public int HotelId { get; set; }
        public int TypWyzywieniaId { get; set; }
        public List<CreateTerminCenaDto> TerminyCeny { get; set; } = new List<CreateTerminCenaDto>();
    }

    public class CreateTerminCenaDto
    {
        public DateTime DataOd { get; set; }
        public DateTime DataDo { get; set; }
        public decimal CenaPodstawowa { get; set; }
    }
}
