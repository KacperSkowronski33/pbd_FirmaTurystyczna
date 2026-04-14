using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.DTOs.Auth
{
    public class LoginDto
    {
        public required string Identifier { get; set; }
        public required string Haslo { get; set; }
    }

    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Wiadomosc { get; set; }
    }
}
