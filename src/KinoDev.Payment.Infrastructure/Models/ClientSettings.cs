using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KinoDev.Payment.Infrastructure.Models
{
    public class ClientSettings
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string SettingsString { get; set; }
    }
}
