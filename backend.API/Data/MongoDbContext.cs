using MongoDB.Driver;
using backend.API.Models;
using backend.API.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Authentication;

namespace backend.API.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<DatabaseSettings> settings)
        {
            try
            {
                var mongoClientSettings = MongoClientSettings.FromConnectionString(settings.Value.ConnectionString);
                
                // Configure SSL settings for Windows compatibility
                if (settings.Value.ConnectionString.Contains("mongodb+srv"))
                {
                    mongoClientSettings.SslSettings = new SslSettings()
                    {
                        EnabledSslProtocols = SslProtocols.Tls12,
                        CheckCertificateRevocation = false
                    };
                    mongoClientSettings.ConnectTimeout = TimeSpan.FromSeconds(30);
                    mongoClientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
                }

                var client = new MongoClient(mongoClientSettings);
                _database = client.GetDatabase(settings.Value.DatabaseName);

                // Test the connection
                _database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}").Wait();
                Console.WriteLine("✅ MongoDB Atlas connection successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MongoDB connection error: {ex.Message}");
                throw;
            }
        }

        public IMongoCollection<Product> Products =>
            _database.GetCollection<Product>(GetCollectionName("Products"));

        private string GetCollectionName(string defaultName)
        {
            // You can add logic here to get collection names from settings if needed
            return defaultName;
        }
    }
}