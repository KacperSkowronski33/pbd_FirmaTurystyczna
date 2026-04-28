using BookingApp.Api.Data;
using BookingApp.Shared.ApiResponse;
using BookingApp.Shared.DTOs.HotelDto;
using BookingApp.Shared.DTOs.OfertaDto;
using BookingApp.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoteleController : ControllerBase
    {
        private readonly BookingDbContext _context;
        public HoteleController(BookingDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadHotelDto>>>> GetHotele()
        {
            var hotele = await _context.Hotele
                .AsNoTracking()
                .Select(h => new ReadHotelDto
                {
                    Id = h.Id,
                    NazwaHotelu = h.NazwaHotelu,
                    LiczbaGwiazdek = h.LiczbaGwiazdek,
                    Adres = h.Adres,
                    NazwaMiejscowosci = h.Miejscowosc != null ? h.Miejscowosc.Nazwa : "brak danych"

                }).ToListAsync();
            return Ok(ApiResponse<IEnumerable<ReadHotelDto>>.Ok(hotele));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Hotel>>> PostHotel(CreateHotelDto dto)
        {
            var nowy = new Hotel
            {
                NazwaHotelu = dto.NazwaHotelu,
                LiczbaGwiazdek = dto.LiczbaGwiazdek,
                MiejscowoscId = dto.MiejscowoscId,
                Adres = dto.Adres,
            };
            _context.Hotele.Add(nowy);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetHotele), new { id = nowy.Id }, ApiResponse<Hotel>.Ok(nowy, "Hotel został dodany"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, UpdateHotelDto dto)
        {
            var hotelBaza = await _context.Hotele.FindAsync(id);
            if(hotelBaza == null)
            {
                return NotFound(ApiResponse.Error($"Hotel o id {id} nie istnieje w bazie"));
            }
            hotelBaza.NazwaHotelu = dto.NazwaHotelu;
            hotelBaza.LiczbaGwiazdek = dto.LiczbaGwiazdek;
            hotelBaza.Adres = dto.Adres;
            hotelBaza.MiejscowoscId = dto.MiejscowoscId;
            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("Hotel został zaktualizowany"));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _context.Hotele.FindAsync(id);
            if(hotel == null)
            {
                return NotFound(ApiResponse.Error($"Hotel o id {id} nie istnieje w bazie"));
            }
            _context.Hotele.Remove(hotel);
            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("Hotel został usunięty"));
        }
    }
}
