namespace KinoDev.Payment.WebApi.Models
{
    public class CreatePaymentModel
    {
        public Guid OrderId { get; set; }
        
        public decimal Amount { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}