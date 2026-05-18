using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using BookingApp.Api.Data;
using BookingApp.Shared.DTOs.RezerwacjaDto;
using Microsoft.EntityFrameworkCore;
using BookingApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RezerwacjeController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly IValidator<CreateRezerwacjaDto> _validator;

        public RezerwacjeController(BookingDbContext context, IValidator<CreateRezerwacjaDto> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostRezerwacja(CreateRezerwacjaDto dto)
        {
            

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                       ?? User.FindFirst("id")
                       ?? User.FindFirst("sub");

            if (idClaim == null || !int.TryParse(idClaim.Value, out int zalogowanyKlientId))
            {
                return Unauthorized("Nie udało się zweryfikować Twojego konta.");
            }

            dto.KlientId = zalogowanyKlientId;

            var wynikWalidacja = await _validator.ValidateAsync(dto);
            if (!wynikWalidacja.IsValid)
            {
                return BadRequest(wynikWalidacja.Errors.Select(e => e.ErrorMessage));
            }

            var termin = await _context.TerminyCeny.FindAsync(dto.TerminCenaId);
            if (termin == null)
            {
                return BadRequest("Wybrany termin nie istnieje.");
            }

            var nowaRezerwacja = new Rezerwacja
            {
                KlientId = zalogowanyKlientId,
                TerminCenaId = dto.TerminCenaId,
                LiczbaOsob = dto.LiczbaOsob,
                KwotaCalkowita = termin.CenaPodstawowa * dto.LiczbaOsob,
                DataUtworzenia = DateTime.UtcNow,
                PracownikId = dto.PracownikId > 0 ? dto.PracownikId : null,
                Pracownik = null,
                Klient = null,
                TerminCena = null
            };

            _context.Rezerwacje.Add(nowaRezerwacja);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                wiadomosc = "Rezerwacja utworzona pomyślnie!",
                id = nowaRezerwacja.Id,
                kwotaDoZaplaty = nowaRezerwacja.KwotaCalkowita
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadRezerwacjaDto>>> GetRezerwacje()
        {
            var rezerwacje = await _context.Rezerwacje
                .Select(r => new ReadRezerwacjaDto
                {
                    Id = r.Id,
                    LiczbaOsob = r.LiczbaOsob,
                    KwotaCalkowita = r.KwotaCalkowita,
                    DataUtworzenia = r.DataUtworzenia
                })
                .ToListAsync();

            return Ok(rezerwacje);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRezerwacja(int id)
        {
            var rezerwacja = await _context.Rezerwacje.FindAsync(id);
            if (rezerwacja == null)
            {
                return NotFound();
            }

            _context.Rezerwacje.Remove(rezerwacja);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("moje")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ReadRezerwacjaDto>>> GetMojeRezerwacje()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                       ?? User.FindFirst("id")
                       ?? User.FindFirst("sub");

            if (idClaim == null || !int.TryParse(idClaim.Value, out int klientId))
            {
                return Unauthorized("Brak poprawnego ID w tokenie.");
            }

            var rezerwacje = await _context.Rezerwacje
                .Where(r => r.KlientId == klientId)
                .Select(r => new ReadRezerwacjaDto
                {
                    Id = r.Id,
                    LiczbaOsob = r.LiczbaOsob,
                    KwotaCalkowita = r.KwotaCalkowita,
                    DataUtworzenia = r.DataUtworzenia
                })
                .ToListAsync();

            return Ok(rezerwacje);
        }
    }
}