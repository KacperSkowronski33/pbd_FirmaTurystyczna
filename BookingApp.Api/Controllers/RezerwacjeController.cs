using BookingApp.Api.Data;
using BookingApp.Shared.DTOs.HotelDto;
using BookingApp.Shared.DTOs.RezerwacjaDto;
using BookingApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RezerwacjeController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public RezerwacjeController(BookingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadRezerwacjaDto>>> GetRezerwacje()
        {
            var rezerwacje = await _context.Rezerwacje
                .AsNoTracking()
                .Select(r => new ReadRezerwacjaDto
                {
                    Id = r.Id,
                    KlientId = r.KlientId,
                    TerminCenaId = r.TerminCenaId,
                    PracownikId = r.PracownikId,
                    LiczbaOsob = r.LiczbaOsob,
                    KwotaCalkowita = r.KwotaCalkowita,
                    DataUtworzenia = r.DataUtworzenia,
                }).ToListAsync();
            return Ok(rezerwacje);
        }

        [HttpPost]
        public async Task<ActionResult<Rezerwacja>> PostRezerwacja(CreateRezerwacjaDto r)
        {
            var nowy = new Rezerwacja
            {
                KlientId = r.KlientId,
                TerminCenaId = r.TerminCenaId,
                PracownikId = r.PracownikId,
                LiczbaOsob = r.LiczbaOsob,
                KwotaCalkowita = r.KwotaCalkowita,
                DataUtworzenia = r.DataUtworzenia,
            };
            _context.Rezerwacje.Add(nowy);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRezerwacje), new { id = nowy.Id }, nowy);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRezerwacja(int id, UpdateRezerwacjaDto r)
        {
            var rezerwacjeBaza = await _context.Rezerwacje.FindAsync(id);
            if (rezerwacjeBaza == null)
            {
                return NotFound($"Rezerwacja o id {id} nie istnieje w bazie");
            }
            rezerwacjeBaza.KlientId = r.KlientId;
            rezerwacjeBaza.TerminCenaId = r.TerminCenaId;
            rezerwacjeBaza.PracownikId = r.PracownikId;
            rezerwacjeBaza.LiczbaOsob = r.LiczbaOsob;
            rezerwacjeBaza.KwotaCalkowita = r.KwotaCalkowita;
            rezerwacjeBaza.DataUtworzenia = r.DataUtworzenia;
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var rezerwacja = await _context.Rezerwacje.FindAsync(id);
            if (rezerwacja == null)
            {
                return NotFound($"Rezerwacja o id = {id} nie istnieje w bazie");
            }
            _context.Rezerwacje.Remove(rezerwacja);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}