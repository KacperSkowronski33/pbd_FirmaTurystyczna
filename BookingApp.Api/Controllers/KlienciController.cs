using BookingApp.Api.Data;
using BookingApp.Shared.DTOs.HotelDto;
using BookingApp.Shared.DTOs.KlientDto;
using BookingApp.Shared.DTOs.RezerwacjaDto;
using BookingApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KlienciController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public KlienciController(BookingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadKlientDto>>> GetKlient()
        {
            var klienci = await _context.Klienci
                .AsNoTracking()
                .Select(k => new ReadKlientDto
                {
                    Id = k.Id,
                    Imie = k.Imie,
                    Nazwisko = k.Nazwisko,
                    Email = k.Email,
                    Haslo = k.Haslo,
                    AdresKlienta = k.AdresKlienta,
                }).ToListAsync();
            return Ok(klienci);
        }
        [HttpPost]
        public async Task<ActionResult<Rezerwacja>> PostKlient(CreateKlientDto k)
        {
            var nowy = new Klient
            {
                Imie = k.Imie,
                Nazwisko = k.Nazwisko,
                Email = k.Email,
                Haslo = k.Haslo,
                AdresKlienta = k.AdresKlienta,
            };
            _context.Klienci.Add(nowy);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetKlient), new { id = nowy.Id }, nowy);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKlient(int id, UpdateKlientDto k)
        {
            var klienciBaza = await _context.Klienci.FindAsync(id);
            if (klienciBaza == null)
            {
                return NotFound($"Rezerwacja o id {id} nie istnieje w bazie");
            }
            klienciBaza.Imie = k.Imie;
            klienciBaza.Nazwisko = k.Nazwisko;
            klienciBaza.Email = k.Email;
            klienciBaza.Haslo = k.Haslo;
            klienciBaza.AdresKlienta = k.AdresKlienta;
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKlient(int id)
        {
            var klient = await _context.Klienci.FindAsync(id);
            if (klient == null)
            {
                return NotFound($"klient o id = {id} nie istnieje w bazie");
            }
            _context.Klienci.Remove(klient);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}