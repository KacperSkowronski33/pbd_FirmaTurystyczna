using BookingApp.Api.Data;
using BookingApp.Shared.ApiResponse;
using BookingApp.Shared.DTOs.HotelDto;
using BookingApp.Shared.DTOs.OfertaDto;
using BookingApp.Shared.DTOs.RezerwacjaDto;
using BookingApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfertyController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public OfertyController(BookingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadOfertaDto>>>> GetOferta()
        {
            var oferty = await _context.Oferty
                .AsNoTracking()
                .Select(o => new ReadOfertaDto
                {
                    Id = o.Id,
                    HotelId = o.HotelId,
                    TypWyzywieniaId = o.TypWyzywieniaId,
                    Hotel = o.Hotel,
                    TypWyzywienia = o.TypWyzywienia,
                    TerminyCeny = o.TerminyCeny,
                }).ToListAsync();
            return Ok(ApiResponse<IEnumerable<ReadOfertaDto>>.Ok(oferty));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Oferta>>> PostOferta(CreateOfertaDto o)
        {
            var nowy = new Oferta
            {
                HotelId = o.HotelId,
                TypWyzywieniaId = o.TypWyzywieniaId,
                Hotel = o.Hotel,
                TypWyzywienia = o.TypWyzywienia,
                TerminyCeny = o.TerminyCeny,
            };
            _context.Oferty.Add(nowy);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOferta), new { id = nowy.Id }, ApiResponse<Oferta>.Ok(nowy, "Oferta została dodana"));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOferta(int id, UpdateOfertaDto o)
        {
            var ofertyBaza = await _context.Oferty.FindAsync(id);
            if (ofertyBaza == null)
            {
                return NotFound(ApiResponse.Error($"Oferta o id {id} nie istnieje w bazie"));
            }
            ofertyBaza.HotelId = o.HotelId;
            ofertyBaza.TypWyzywieniaId = o.TypWyzywieniaId;
            ofertyBaza.Hotel = o.Hotel;
            ofertyBaza.TypWyzywienia = o.TypWyzywienia;
            ofertyBaza.TerminyCeny = o.TerminyCeny;
            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("Oferta została zaktualizowana"));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOferta(int id)
        {
            var Oferta = await _context.Oferty.FindAsync(id);
            if (Oferta == null)
            {
                return NotFound(ApiResponse.Error($"Oferta o id {id} nie istnieje w bazie"));
            }
            _context.Oferty.Remove(Oferta);
            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("Oferta została usunięta"));
        }
    }
}