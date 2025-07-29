using MongoDB.Driver;
using ClothingPOS.API.Models;
using ClothingPOS.API.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace ClothingPOS.API.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<DatabaseSettings> databaseSettings)
        {
            var settings = MongoClientSettings.FromConnectionString(databaseSettings.Value.ConnectionString);
            
            // Configure SSL settings for MongoDB Atlas
            settings.SslSettings = new SslSettings
            {
                EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
            };
            
            // Set connection timeout
            settings.ConnectTimeout = TimeSpan.FromSeconds(30);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
            
            var client = new MongoClient(settings);
            _database = client.GetDatabase(databaseSettings.Value.DatabaseName);
        }

        public IMongoCollection<Product> Products =>
            _database.GetCollection<Product>("Products");

        public IMongoCollection<Sale> Sales =>
            _database.GetCollection<Sale>("Sales");
    }
}
