using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using BookingApp.Api.Data;
using BookingApp.Shared.DTOs.RezerwacjaDto;
using Microsoft.EntityFrameworkCore;
using BookingApp.Shared.Models;

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
        public async Task<ActionResult> PostRezerwacja(CreateRezerwacjaDto dto)
        {
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
                KlientId = dto.KlientId,
                TerminCenaId = dto.TerminCenaId,
                PracownikId = dto.PracownikId,
                LiczbaOsob = dto.LiczbaOsob,
                KwotaCalkowita = termin.CenaPodstawowa * dto.LiczbaOsob,
                DataUtworzenia = DateTime.Now

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
    }
}
