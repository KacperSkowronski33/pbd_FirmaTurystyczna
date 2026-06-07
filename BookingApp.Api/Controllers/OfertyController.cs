using BookingApp.Api.Data;
using BookingApp.Shared.ApiResponse;
using BookingApp.Shared.DTOs.HotelDto;
using BookingApp.Shared.DTOs.OfertaDto;
using BookingApp.Shared.DTOs.RezerwacjaDto;
using BookingApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadOfertaDto>>>> GetOferta(
            [FromQuery] string? cel,
            [FromQuery] decimal? maxCena,
            [FromQuery] string? gwiazdki,
            [FromQuery] int? typWyzywieniaId,
            [FromQuery] bool tylkoAktywne = true) 
        {
            var query = _context.Oferty.AsNoTracking();

            if (tylkoAktywne)
            {
                query = query.Where(o => o.CzyAktywna);
            }

            if (!string.IsNullOrEmpty(cel))
            {
                query = query.Where(o => o.Hotel.NazwaHotelu.ToLower().Contains(cel.ToLower()) ||
                                         o.Hotel.Miejscowosc.Nazwa.ToLower().Contains(cel.ToLower()));
            }
            if (maxCena.HasValue)
            {
                query = query.Where(o => o.TerminyCeny.Any(t => t.CenaPodstawowa <= maxCena.Value));
            }
            if (!string.IsNullOrEmpty(gwiazdki))
            {
                var listaGwiazdek = gwiazdki.Split(',').Select(s => int.TryParse(s, out var n) ? n : (int?)null).Where(n => n.HasValue).Select(n => n!.Value).ToList();
                if (listaGwiazdek.Any()) query = query.Where(o => listaGwiazdek.Contains(o.Hotel.LiczbaGwiazdek));
            }
            if (typWyzywieniaId.HasValue)
            {
                query = query.Where(o => o.TypWyzywieniaId == typWyzywieniaId.Value);
            }

            var oferty = await query
                .Select(o => new ReadOfertaDto
                {
                    Id = o.Id,
                    HotelId = o.HotelId,
                    TypWyzywieniaId = o.TypWyzywieniaId,
                    TypWyzywienia = o.TypWyzywienia,
                    TerminyCeny = o.TerminyCeny.ToList(),
                    CzyAktywna = o.CzyAktywna, 
                    Hotel = new Hotel
                    {
                        Id = o.Hotel.Id,
                        NazwaHotelu = o.Hotel.NazwaHotelu,
                        LiczbaGwiazdek = o.Hotel.LiczbaGwiazdek,
                        MiejscowoscId = o.Hotel.MiejscowoscId,
                        Miejscowosc = o.Hotel.Miejscowosc,
                        ZdjeciaHotelu = o.Hotel.ZdjeciaHotelu.ToList()
                    }
                }).ToListAsync();

            return Ok(ApiResponse<IEnumerable<ReadOfertaDto>>.Ok(oferty));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ReadOfertaDto>>> GetOferta(int id)
        {
            var oferta = await _context.Oferty
                .AsNoTracking()
                .Where(o => o.Id == id)
                .Select(o => new ReadOfertaDto
                {
                    Id = o.Id,
                    HotelId = o.HotelId,
                    TypWyzywieniaId = o.TypWyzywieniaId,
                    TypWyzywienia = o.TypWyzywienia,
                    TerminyCeny = o.TerminyCeny.ToList(),
                    CzyAktywna = o.CzyAktywna, 
                    Hotel = new Hotel
                    {
                        Id = o.Hotel.Id,
                        NazwaHotelu = o.Hotel.NazwaHotelu,
                        LiczbaGwiazdek = o.Hotel.LiczbaGwiazdek,
                        MiejscowoscId = o.Hotel.MiejscowoscId,
                        Miejscowosc = o.Hotel.Miejscowosc,
                        ZdjeciaHotelu = o.Hotel.ZdjeciaHotelu.ToList()
                    }
                })
                .FirstOrDefaultAsync();

            if (oferta == null)
            {
                return NotFound(ApiResponse.Error($"Oferta o id {id} nie istnieje w bazie"));
            }

            return Ok(ApiResponse<ReadOfertaDto>.Ok(oferta));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Oferta>>> PostOferta(CreateOfertaDto o)
        {
            var hotelIstnieje = await _context.Hotele.AnyAsync(h => h.Id == o.HotelId);
            if (!hotelIstnieje) return BadRequest(ApiResponse.Error($"Wybrany hotel o ID {o.HotelId} nie istnieje w bazie."));

            var wyzywienieIstnieje = await _context.TypyWyzywienia.AnyAsync(w => w.Id == o.TypWyzywieniaId);
            if (!wyzywienieIstnieje) return BadRequest(ApiResponse.Error($"Wybrany typ wyżywienia o ID {o.TypWyzywieniaId} nie istnieje w bazie."));

            var nowy = new Oferta
            {
                HotelId = o.HotelId,
                TypWyzywieniaId = o.TypWyzywieniaId,
                CzyAktywna = true, 
                TerminyCeny = o.TerminyCeny.Select(t => new TerminCena
                {
                    DataOd = t.DataOd,
                    DataDo = t.DataDo,
                    CenaPodstawowa = t.CenaPodstawowa
                }).ToList()
            };

            _context.Oferty.Add(nowy);
            await _context.SaveChangesAsync();

            foreach (var termin in nowy.TerminyCeny)
            {
                termin.Oferta = null!;
            }

            return Ok(ApiResponse<Oferta>.Ok(nowy, "Oferta została dodana"));
        }

        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "Pracownik,Administrator")]
        public async Task<IActionResult> ToggleActiveStatus(int id)
        {
            var oferta = await _context.Oferty.FindAsync(id);
            if (oferta == null)
            {
                return NotFound(ApiResponse.Error($"Oferta o id {id} nie istnieje w bazie."));
            }

            oferta.CzyAktywna = !oferta.CzyAktywna; 
            await _context.SaveChangesAsync();

            string stan = oferta.CzyAktywna ? "widoczna dla klientów" : "ukryta (dostępna w raportach)";
            return Ok(ApiResponse.Ok($"Status oferty został zmieniony. Obecnie jest: {stan}."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Pracownik,Administrator")]
        public async Task<IActionResult> DeleteOferta(int id)
        {
            var Oferta = await _context.Oferty.FindAsync(id);
            if (Oferta == null)
            {
                return NotFound(ApiResponse.Error($"Oferta o id {id} nie istnieje w bazie"));
            }
            _context.Oferty.Remove(Oferta);
            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("Oferta została trwale usunięta z bazy danych"));
        }
    }
}