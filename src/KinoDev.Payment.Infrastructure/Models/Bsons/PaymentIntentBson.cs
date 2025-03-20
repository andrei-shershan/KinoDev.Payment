using KinoDev.Payment.Infrastructure.Models.PaymentIntents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KinoDev.Payment.Infrastructure.Models.Bsons
{
    public class PaymentIntentBson
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string PaymentIntentId { get; set; }

        public decimal Amount { get; set; }

        public string ClientSecret { get; set; }

        public string Currency { get; set; }

        [BsonElement("Metadata")]
        public Dictionary<string, string> Metadata { get; set; }

        [BsonRepresentation(BsonType.String)]
        public PaymentProvider PaymentProvider { get; set; }
    }
}