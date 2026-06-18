using BookingApp.Api.Data;
using BookingApp.Api.Models.PayU;
using BookingApp.Shared.ApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BookingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayUController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly IConfiguration _configuration;

        public PayUController(BookingDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("order/{rezerwacjaId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> InicjujPlatnosc(int rezerwacjaId)
        {
            var rezerwacja = await _context.Rezerwacje
                .Include(r => r.Klient)
                .Include(r => r.TerminCena)
                .ThenInclude(tc => tc.Oferta)
                .ThenInclude(o => o.Hotel)
                .FirstOrDefaultAsync(r => r.Id == rezerwacjaId);

            if (rezerwacja == null) return NotFound(ApiResponse<string>.Error("Nie znaleziono wybranej rezerwacji."));

            if (rezerwacja.StatusPlatnosci == "Completed")
            {
                return BadRequest(ApiResponse<string>.Error("Ta rezerwacja została już wcześniej opłacona."));
            }

            try
            {
                string accessToken = await PobierzTokenOAuthAsync();

                var handler = new HttpClientHandler { AllowAutoRedirect = false };
                using var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                int kwotaWGroszach = (int)(rezerwacja.KwotaCalkowita * 100);
                var payuRequest = new PayUOrderRequest
                {
                    NotifyUrl = _configuration["PayU:NotifyUrl"] ?? "",
                    CustomerIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                    MerchantPosId = _configuration["PayU:MerchantPosId"] ?? "",
                    Description = $"Rezerwacja wycieczki nr {rezerwacja.Id} - {rezerwacja.TerminCena.Oferta.Hotel.NazwaHotelu}",
                    TotalAmount = kwotaWGroszach.ToString(),
                    ExtOrderId = rezerwacja.Id.ToString(),
                    Buyer = new PayUBuyer
                    {
                        Email = string.IsNullOrWhiteSpace(rezerwacja.Klient.Email) ? "test@example.com" : rezerwacja.Klient.Email,
                        FirstName = string.IsNullOrWhiteSpace(rezerwacja.Klient.Imie) ? "Jan" : rezerwacja.Klient.Imie,
                        LastName = string.IsNullOrWhiteSpace(rezerwacja.Klient.Nazwisko) ? "Kowalski" : rezerwacja.Klient.Nazwisko
                    },
                    Products = new List<PayUProduct>
                    {
                        new PayUProduct
                        {
                            Name = $"Wyjazd wypoczynkowy: {rezerwacja.TerminCena.Oferta.Hotel.NazwaHotelu}",
                            UnitPrice = kwotaWGroszach.ToString()
                        }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payuRequest);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var url = $"{_configuration["PayU:BaseUrl"]}/api/v2_1/orders";
                var response = await client.PostAsync(url, content);

                string redirectUri = string.Empty;
                string payuOrderId = string.Empty;

                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.Found)
                {
                    redirectUri = response.Headers.Location?.AbsoluteUri ?? string.Empty;
                }
                else if (response.IsSuccessStatusCode) 
                {
                    var payuResponse = JsonSerializer.Deserialize<PayUOrderResponse>(responseBody);
                    if (payuResponse != null)
                    {
                        redirectUri = payuResponse.RedirectUri;
                        payuOrderId = payuResponse.OrderId;
                    }
                }
                else
                {
                    return BadRequest(ApiResponse<string>.Error($"Serwer PayU odrzucił żądanie (HTTP {(int)response.StatusCode}): {responseBody}"));
                }

                if (string.IsNullOrEmpty(redirectUri))
                {
                    return BadRequest(ApiResponse<string>.Error($"PayU nie zwróciło linku przekierowania. Odpowiedź: {responseBody}"));
                }

                if (!string.IsNullOrEmpty(payuOrderId))
                {
                    rezerwacja.PayUOrderId = payuOrderId;
                    await _context.SaveChangesAsync();
                }

                return Ok(ApiResponse<string>.Ok(redirectUri));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Error($"Błąd krytyczny integracji PayU: {ex.Message}"));
            }
        }

        [HttpPost("notify")]
        [AllowAnonymous]
        public async Task<IActionResult> OdbierzPowiadomienie()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            string rawBody = await reader.ReadToEndAsync();

            string signatureHeader = Request.Headers["OpenPayU-Signature"].ToString();

            if (string.IsNullOrEmpty(signatureHeader) || string.IsNullOrEmpty(rawBody))
            {
                return BadRequest("Brak podpisu lub zawartości żądania.");
            }

            string secondKey = _configuration["PayU:Md5SecondKey"] ?? "";
            if (!WalidujSygnaturePayU(signatureHeader, rawBody, secondKey))
            {
                return Unauthorized("Sygnatura żądania PayU jest niepoprawna!");
            }

            try
            {
                var payload = JsonSerializer.Deserialize<PayUNotificationPayload>(rawBody);
                if (payload == null || payload.Order == null) return BadRequest("Niepoprawny format danych.");

                if (int.TryParse(payload.Order.ExtOrderId, out int rezerwacjaId))
                {
                    var rezerwacja = await _context.Rezerwacje.FirstOrDefaultAsync(r => r.Id == rezerwacjaId);
                    if (rezerwacja != null)
                    {
                        if (payload.Order.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase))
                        {
                            rezerwacja.StatusPlatnosci = "Completed";
                            rezerwacja.PayUOrderId = payload.Order.OrderId;
                        }
                        else if (payload.Order.Status.Equals("CANCELED", StringComparison.OrdinalIgnoreCase))
                        {
                            rezerwacja.StatusPlatnosci = "Canceled";
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        private async Task<string> PobierzTokenOAuthAsync()
        {
            using var client = new HttpClient();
            var dict = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _configuration["PayU:ClientId"] ?? "" },
                { "client_secret", _configuration["PayU:ClientSecret"] ?? "" }
            };

            var req = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["PayU:BaseUrl"]}/pl/standard/user/oauth/authorize")
            {
                Content = new FormUrlEncodedContent(dict)
            };

            var res = await client.SendAsync(req);
            if (!res.IsSuccessStatusCode)
            {
                string errorBody = await res.Content.ReadAsStringAsync();
                throw new Exception($"Autoryzacja OAuth nie powiodła się. Kod: {res.StatusCode}, Szczegóły: {errorBody}");
            }

            var body = await res.Content.ReadAsStringAsync();
            var authData = JsonSerializer.Deserialize<PayUAuthResponse>(body);

            return authData?.AccessToken ?? string.Empty;
        }

        private bool WalidujSygnaturePayU(string header, string body, string secondKey)
        {
            try
            {
                var parts = header.Split(';');
                var sigPart = parts.FirstOrDefault(p => p.Trim().StartsWith("signature="));
                if (sigPart == null) return false;

                string payuHash = sigPart.Split('=')[1].Trim();
                string inputToHash = body + secondKey;

                using var md5 = MD5.Create();
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputToHash);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (byte b in hashBytes) sb.Append(b.ToString("x2"));

                return sb.ToString().Equals(payuHash, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}