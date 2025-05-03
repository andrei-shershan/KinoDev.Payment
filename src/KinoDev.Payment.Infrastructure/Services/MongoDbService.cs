using KinoDev.Payment.Infrastructure.Abstractions;
using KinoDev.Payment.Infrastructure.Models;
using KinoDev.Payment.Infrastructure.Models.Bsons;
using KinoDev.Shared.DtoModels.PaymentIntents;
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

        public async Task<IEnumerable<GenericPaymentIntent>> GetOrderPaymentIntentsAsync(Guid orderId)
        {
            var filter = Builders<PaymentIntentBson>.Filter.Eq(x => x.OrderId, orderId.ToString());

            var paymentIntents = await _paymentIntentsCollection.Find(filter).ToListAsync();

            return paymentIntents?.Select(x => new GenericPaymentIntent
            {
                PaymentIntentId = x.PaymentIntentId,
                OrderId = Guid.Parse(x.OrderId),
                ClientSecret = x.ClientSecret,
                Amount = x.Amount,
                Currency = x.Currency,
                Metadata = x.Metadata,
                State = x.State,
                PaymentProvider = x.PaymentProvider
            });
        }

        public async Task<GenericPaymentIntent> GetPaymentIntentAsync(string paymentIntentId)
        {
            var filter = Builders<PaymentIntentBson>.Filter.Eq(x => x.PaymentIntentId, paymentIntentId);

            var paymentIntent = await _paymentIntentsCollection.Find(filter).FirstOrDefaultAsync();
            if (paymentIntent == null)
            {
                return null;
            }

            return new GenericPaymentIntent
            {
                PaymentIntentId = paymentIntent.PaymentIntentId,
                OrderId = Guid.Parse(paymentIntent.OrderId),
                ClientSecret = paymentIntent.ClientSecret,
                Amount = paymentIntent.Amount,
                Currency = paymentIntent.Currency,
                Metadata = paymentIntent.Metadata,
                State = paymentIntent.State,
                PaymentProvider = paymentIntent.PaymentProvider
            };
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
                State = paymentIntent.State,
                OrderId = paymentIntent.OrderId.ToString()
            };

            return _paymentIntentsCollection.InsertOneAsync(bsonPaymentIntent);
        }

        public async Task UpdatePaymentIntentStateAsync(string paymentIntentId, string state)
        {
            var filter = Builders<PaymentIntentBson>.Filter.Eq(x => x.PaymentIntentId, paymentIntentId);
            var update = Builders<PaymentIntentBson>.Update.Set(x => x.State, state);

            await _paymentIntentsCollection.UpdateOneAsync(filter, update);
        }
    }
}