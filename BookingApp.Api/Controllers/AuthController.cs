using BookingApp.Api.Data;
using BookingApp.Shared.DTOs.Auth;
using BookingApp.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;


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

            if (!loginDto.Identifier.Contains("@"))
            {
                var pracownik = await _context.Pracownicy
                    .Include(p => p.Rola)
                    .FirstOrDefaultAsync(p => p.Email == loginDto.Identifier);
                if (pracownik == null || !BCrypt.Net.BCrypt.Verify(loginDto.Haslo, pracownik.Haslo))
                {
                    return Unauthorized(new LoginResultDto { Success = false, Wiadomosc = "Niepoprawny login lub hasło"});
                }
                rola = pracownik.Rola.TypRoli;
                userMail = pracownik.Email;
                userName = $"{pracownik.Imie} {pracownik.Nazwisko}";
            } else
            {
                var klient = await _context.Klienci
                    .FirstOrDefaultAsync(k => k.Email == loginDto.Identifier);

                if(klient == null || !BCrypt.Net.BCrypt.Verify(loginDto.Haslo, klient.Haslo))
                { 
                    return Unauthorized(new LoginResultDto { Success = false, Wiadomosc = "Niepoprawny login lub hasło" });
                }

                rola = TypRoli.Brak;
                userMail = klient.Email;
                userName = $"{klient.Imie} {klient.Nazwisko}";

            }

            var token = GenJwtToken(userMail, rola, userName);
            return Ok(new LoginResultDto { Success = true, Token = token });

        }

        private string GenJwtToken(string Mail, TypRoli Rola, string Name)
        {

            var claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name, Name),
                new Claim(ClaimTypes.Email, Mail),
                new Claim(ClaimTypes.Role, Rola.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(1);

            var Token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
    }
}
