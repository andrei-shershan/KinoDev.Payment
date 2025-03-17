namespace KinoDev.Payment.WebApi.Models
{
    public class CreatePaymentIntentModel
    {
        public string Currency { get; set; }

        public int Amount { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}