using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.DTOs.Auth
{
    public class RegisterDto
    {
        public required string Imie {  get; set; }
        public required string Nazwisko { get; set; }
        public required string Email { get; set; }
        public required string Haslo { get; set; }

    }
}
