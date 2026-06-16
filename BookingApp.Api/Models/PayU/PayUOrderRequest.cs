using System.Text.Json.Serialization;

namespace BookingApp.Api.Models.PayU
{
    public class PayUOrderRequest
    {
        [JsonPropertyName("notifyUrl")]
        public string NotifyUrl { get; set; } = string.Empty;

        [JsonPropertyName("customerIp")]
        public string CustomerIp { get; set; } = string.Empty;

        [JsonPropertyName("merchantPosId")]
        public string MerchantPosId { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; } = "PLN";

        [JsonPropertyName("totalAmount")]
        public string TotalAmount { get; set; } = string.Empty; // W groszach, np. "150000" (1500,00 zł)

        [JsonPropertyName("extOrderId")]
        public string ExtOrderId { get; set; } = string.Empty; //wewnętrzne ID rezerwacji

        [JsonPropertyName("buyer")]
        public PayUBuyer Buyer { get; set; } = new PayUBuyer();

        [JsonPropertyName("products")]
        public List<PayUProduct> Products { get; set; } = new List<PayUProduct>();
    }
    public class PayUBuyer
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
    }

    public class PayUProduct
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("unitPrice")]
        public string UnitPrice { get; set; } = string.Empty; // Grosze

        [JsonPropertyName("quantity")]
        public string Quantity { get; set; } = "1";
    }
}
