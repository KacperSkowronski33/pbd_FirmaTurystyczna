using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Shared.ApiResponse
{
    public class ApiResponse
    {
        public bool Sukces { get; set; }
        public string Wiadomosc { get; set; } = string.Empty;

        public static ApiResponse Ok(string message = "") => new ApiResponse { Sukces = true, Wiadomosc = message };
        public static ApiResponse Error(string message) => new ApiResponse { Sukces = false, Wiadomosc = message };
    }
    public class ApiResponse<T> : ApiResponse
    {
        public T? Dane { get; set; }

        public static ApiResponse<T> Ok(T dane, string wiad = "")
        {
            return new ApiResponse<T> { Sukces = true, Dane = dane, Wiadomosc = wiad };
        }

        public new static ApiResponse<T> Error(string wiad)
        {
            return new ApiResponse<T> { Sukces = false, Wiadomosc = wiad };
        }
    }
}
