using BookingApp.Api.Data;
using BookingApp.Shared.ApiResponse;
using BookingApp.Shared.DTOs.OfertaDto;
using BookingApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UlubioneController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public UlubioneController(BookingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadOfertaDto>>>> GetUlubione()
        {
            var klientId = PobierzZalogowanegoKlientaId();
            if (klientId == null) return Unauthorized(ApiResponse.Error("Niepoprawny token autoryzacji."));

            var oferty = await _context.UlubioneOferty
                .AsNoTracking()
                .Where(u => u.KlientId == klientId.Value && u.Oferta.CzyAktywna) 
                .Select(u => u.Oferta)
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
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<ReadOfertaDto>>.Ok(oferty));
        }

        [HttpGet("ids")]
        public async Task<ActionResult<ApiResponse<IEnumerable<int>>>> GetUlubioneIds()
        {
            var klientId = PobierzZalogowanegoKlientaId();
            if (klientId == null) return Unauthorized(ApiResponse.Error("Niepoprawny token autoryzacji."));

            var ids = await _context.UlubioneOferty
                .AsNoTracking()
                .Where(u => u.KlientId == klientId.Value)
                .Select(u => u.OfertaId)
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<int>>.Ok(ids));
        }

        [HttpPost("{ofertaId}")]
        public async Task<ActionResult<ApiResponse>> DodajDoUlubionych(int ofertaId)
        {
            var klientId = PobierzZalogowanegoKlientaId();
            if (klientId == null) return Unauthorized(ApiResponse.Error("Niepoprawny token autoryzacji."));

            var ofertaIstnieje = await _context.Oferty.AnyAsync(o => o.Id == ofertaId);
            if (!ofertaIstnieje) return NotFound(ApiResponse.Error("Wybrana oferta nie istnieje w bazie."));

            var juzPolubione = await _context.UlubioneOferty
                .AnyAsync(u => u.KlientId == klientId.Value && u.OfertaId == ofertaId);

            if (juzPolubione)
            {
                return BadRequest(ApiResponse.Error("Ta oferta znajduje się już w Twoich ulubionych."));
            }

            var nowaRelacja = new UlubionaOferta
            {
                KlientId = klientId.Value,
                OfertaId = ofertaId
            };

            _context.UlubioneOferty.Add(nowaRelacja);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("Oferta została pomyślnie dodana do ulubionych."));
        }

        [HttpDelete("{ofertaId}")]
        public async Task<ActionResult<ApiResponse>> UsunZUlubionych(int ofertaId)
        {
            var klientId = PobierzZalogowanegoKlientaId();
            if (klientId == null) return Unauthorized(ApiResponse.Error("Niepoprawny token autoryzacji."));

            var relacja = await _context.UlubioneOferty
                .FirstOrDefaultAsync(u => u.KlientId == klientId.Value && u.OfertaId == ofertaId);

            if (relacja == null)
            {
                return NotFound(ApiResponse.Error("Ta oferta nie znajduje się na Twojej liście ulubionych."));
            }

            _context.UlubioneOferty.Remove(relacja);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("Oferta została usunięta z ulubionych."));
        }

        private int? PobierzZalogowanegoKlientaId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                       ?? User.FindFirst("id")
                       ?? User.FindFirst("sub");

            if (idClaim != null && int.TryParse(idClaim.Value, out int id))
            {
                return id;
            }
            return null;
        }
    }
}