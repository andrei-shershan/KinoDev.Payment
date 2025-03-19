using KinoDev.Payment.Infrastructure.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace KinoDev.Payment.Infrastructure.Services
{
    public interface IMongoDbService
    {
        Task Foo(string message);
    }

    public class MessageModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Message { get; set; }
    }

    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IOptions<MongoDbConfiguration> mongoDbConfiguration)
        {
            var client = new MongoClient(mongoDbConfiguration.Value.ConnectionString);
            _database = client.GetDatabase(mongoDbConfiguration.Value.DatabaseName);
        }

        public async Task Foo(string message)
        {
            // Check if collection exists, create if not
            var filter = new BsonDocument("name", "Messages");
            var collections = _database.ListCollections(new ListCollectionsOptions { Filter = filter });

            if (!await collections.AnyAsync())
            {
                System.Console.WriteLine("Creating Messages collection");
                await _database.CreateCollectionAsync("Messages");
            }

            var messagesCollection = _database.GetCollection<MessageModel>("Messages");

            var id = ObjectId.GenerateNewId().ToString();
            await messagesCollection.InsertOneAsync(new MessageModel
            {
                Id = id,
                Message = message
            });

            var res = await messagesCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

            System.Console.WriteLine(res?.ToString());
        }
    }
}