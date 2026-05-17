using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using BookingApp.Api.Data;
using BookingApp.Shared.ApiResponse;
using BookingApp.Shared.DTOs.Auth;
using BookingApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace BookingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(BookingDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginDto loginDto)
        {
            TypRoli rola = TypRoli.Brak;
            string userMail = "";
            string userName = "";
            int userId = 0;

            if (!loginDto.Identifier.Contains("@"))
            {
                var pracownik = await _context.Pracownicy
                    .Include(p => p.Rola)
                    .FirstOrDefaultAsync(p => p.Email == loginDto.Identifier);

                if (pracownik == null || pracownik.Haslo != loginDto.Haslo)
                {
                    return Unauthorized(ApiResponse<LoginResultDto>.Error("Niepoprawny login lub hasło"));
                }
                rola = pracownik.Rola.TypRoli;
                userMail = pracownik.Email;
                userName = $"{pracownik.Imie} {pracownik.Nazwisko}";
                userId = pracownik.Id;
            }
            else
            {
                var klient = await _context.Klienci
                    .FirstOrDefaultAsync(k => k.Email == loginDto.Identifier);

                if (klient == null || klient.Haslo != loginDto.Haslo)
                {
                    return Unauthorized(ApiResponse<LoginResultDto>.Error("Niepoprawny login lub hasło"));
                }
                rola = TypRoli.Klient;
                userMail = klient.Email;
                userName = $"{klient.Imie} {klient.Nazwisko}";
                userId = klient.Id;
            }

            var token = GenJwtToken(userId, userMail, rola, userName);
            return Ok(ApiResponse<LoginResultDto>.Ok(new LoginResultDto { Token = token }, "Zalogowano pomyślnie"));
        }

        private string GenJwtToken(int Id, string Mail, TypRoli Rola, string Name)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Name, Name),
                new Claim(ClaimTypes.Email, Mail),
                new Claim(ClaimTypes.Role, Rola.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(30);

            var Token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterDto register)
        {
            var emailIstniejeWBazie = await _context.Klienci.AnyAsync(k => k.Email.ToLower() == register.Email.ToLower());
            if (emailIstniejeWBazie)
            {
                return BadRequest(ApiResponse.Error("Podany adres email istnieje już w bazie"));
            }
            var noweKonto = new Klient
            {
                Imie = register.Imie,
                Nazwisko = register.Nazwisko,
                Email = register.Email,
                //Haslo = BCrypt.Net.BCrypt.HashPassword(register.Haslo)
                Haslo = register.Haslo,

            };
            _context.Klienci.Add(noweKonto);
            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("Rejestracja przebiegła pomyślnie."));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rola = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                return Unauthorized(ApiResponse.Error("Brak dostępu."));
            }
            if(rola != "Brak")
            {
                var pracownik = await _context.Pracownicy.FindAsync(id);
                return Ok(ApiResponse<object>.Ok(new { pracownik.Imie, pracownik.Nazwisko, pracownik.Email, Rola = rola }));
            } else
            {
                var klient = await _context.Klienci.FindAsync(id);
                return Ok(ApiResponse<object>.Ok(new { klient.Imie, klient.Nazwisko, klient.Email, Rola = "Klient" }));
            }
        }
    }
}
