namespace KinoDev.Payment.WebApi.Models
{
    public class CreatePaymentModel
    {
        public Guid OrderId { get; set; }
        
        public string Currency { get; set; } = "usd";

        public decimal Amount { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}