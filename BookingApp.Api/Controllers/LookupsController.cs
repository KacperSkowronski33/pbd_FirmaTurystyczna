using BookingApp.Api.Data;
using BookingApp.Shared.DTOs.Lookups;
using BookingApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupsController : ControllerBase
    {
        public readonly BookingDbContext _context;
        public LookupsController(BookingDbContext context)
        {
            _context = context;
        }

        //kraje

        [HttpGet("kraje")]
        public async Task<ActionResult<IEnumerable<KrajDto>>> GetKraje()
        {
            var kraje = await _context.Kraje
                .AsNoTracking()
                .Select(k => new KrajDto
                {
                    Id = k.Id,
                    Nazwa = k.Nazwa,
                })
                .ToListAsync();
            return Ok(kraje);
        }
        [HttpPost("kraje")]
        public async Task<ActionResult> PostKraj(CreateKrajDto dto)
        {
            var nowyKraj = new Kraj { Nazwa = dto.Nazwa };
            _context.Add(nowyKraj);
            await _context.SaveChangesAsync();
            //return Ok($"Kraj {nowyKraj.Nazwa} został utowrzony - id {nowyKraj.Id}"); - wyrzucilby 200ok , created wyrzuci 201created
            return CreatedAtAction(nameof(GetKraje), new { id = nowyKraj.Id }, nowyKraj);
        }

        //miejscowosci

        [HttpGet("miejscowosci")]
        public async Task<ActionResult<IEnumerable<MiejscowoscDto>>> GetMiejscowosci()
        {
            var miejscowosci = await _context.Miejscowsci
                .AsNoTracking()
                .Select(m => new MiejscowoscDto
                {
                    Id = m.Id,
                    Nazwa = m.Nazwa,
                    KrajId = m.KrajId,
                })
                .ToListAsync();
            return Ok(miejscowosci);
        }

        [HttpPost("miejscowosci")]
        public async Task<ActionResult> PostMiejscowosc(CreateMiejscowoscDto dto)
        {
            var krajIstnieje = await _context.Kraje.AnyAsync(k => k.Id == dto.KrajId);
            if(!krajIstnieje)
            {
                return BadRequest($"Kraj o id {dto.KrajId} nie istnieje w bazie!");
            }
            var nowaMiejscowosc = new Miejscowosc
            {
                Nazwa = dto.Nazwa,
                KrajId = dto.KrajId
            };

            _context.Miejscowsci.Add(nowaMiejscowosc);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMiejscowosci), new { id = nowaMiejscowosc.Id }, nowaMiejscowosc);
        }
        
    }
}
