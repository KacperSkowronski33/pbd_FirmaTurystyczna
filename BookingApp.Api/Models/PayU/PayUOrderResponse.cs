using System.Text.Json.Serialization;

namespace BookingApp.Api.Models.PayU
{
    public class PayUOrderResponse
    {
        [JsonPropertyName("redirectUri")]
        public string RedirectUri { get; set; } = string.Empty;

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; } = string.Empty; // ID transakcji PayU
    }

    // payload z payu
    public class PayUNotificationPayload
    {
        [JsonPropertyName("order")]
        public PayUNotificationOrder Order { get; set; } = new PayUNotificationOrder();
    }

    public class PayUNotificationOrder
    {
        [JsonPropertyName("orderId")]
        public string OrderId { get; set; } = string.Empty;

        [JsonPropertyName("extOrderId")]
        public string ExtOrderId { get; set; } = string.Empty; // ID rezerwacji

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty; // COMPLETED, CANCELED, PENDING itd.
    }
}
