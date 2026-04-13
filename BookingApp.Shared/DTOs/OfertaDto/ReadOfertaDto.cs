using BookingApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.DTOs.OfertaDto
{
    public class ReadOfertaDto
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int TypWyzywieniaId { get; set; }
        public Hotel Hotel { get; set; }
        public TypWyzywienia TypWyzywienia { get; set; }
        public ICollection<TerminCena> TerminyCeny { get; set; } = new List<TerminCena>();
    }
}
