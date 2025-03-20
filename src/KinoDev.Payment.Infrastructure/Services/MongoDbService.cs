using KinoDev.Payment.Infrastructure.Models;
using KinoDev.Payment.Infrastructure.Models.Bsons;
using KinoDev.Payment.Infrastructure.Models.PaymentIntents;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KinoDev.Payment.Infrastructure.Services
{
    public class MongoDbService : IDbService
    {
        private readonly IMongoDatabase _database;

        private readonly string _collectionName = "PaymentIntents";

        private readonly IMongoCollection<PaymentIntentBson> _paymentIntentsCollection;

        public MongoDbService(IOptions<MongoDbConfiguration> mongoDbConfiguration)
        {
            var client = new MongoClient(mongoDbConfiguration.Value.ConnectionString);
            _database = client.GetDatabase(mongoDbConfiguration.Value.DatabaseName);

            // Check if collection exists, create if not
            var filter = new BsonDocument("name", _collectionName);
            var collections = _database.ListCollections(new ListCollectionsOptions { Filter = filter });

            if (!collections.Any())
            {
                _database.CreateCollection(_collectionName);
            }

            _paymentIntentsCollection = _database.GetCollection<PaymentIntentBson>(_collectionName);
        }

        public Task SavePaymentIntentAsync(GenericPaymentIntent paymentIntent)
        {
            var bsonPaymentIntent = new PaymentIntentBson
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Amount = paymentIntent.Amount,
                Currency = paymentIntent.Currency,
                Metadata = paymentIntent.Metadata,
                PaymentProvider = paymentIntent.PaymentProvider,
                PaymentIntentId = paymentIntent.PaymentIntentId,
                ClientSecret = paymentIntent.ClientSecret,
            };

            return _paymentIntentsCollection.InsertOneAsync(bsonPaymentIntent);
        }
    }
}