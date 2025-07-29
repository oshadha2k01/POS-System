using ClothingPOS.API.Data;
using ClothingPOS.API.Services;
using ClothingPOS.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure MongoDB
builder.Services.Configure<DatabaseSettings>(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
    options.DatabaseName = builder.Configuration.GetValue<string>("DatabaseSettings:DatabaseName") ?? "ClothingPOSDB";
    options.ProductsCollectionName = builder.Configuration.GetValue<string>("DatabaseSettings:ProductsCollectionName") ?? "Products";
    options.SalesCollectionName = builder.Configuration.GetValue<string>("DatabaseSettings:SalesCollectionName") ?? "Sales";
});

// Register MongoDB Context
builder.Services.AddSingleton<MongoDbContext>();

// Register Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISaleService, SaleService>();

// Add HttpClient for AI Model integration (optional)
builder.Services.AddHttpClient<ForecastService>();

// Add custom services
builder.Services.AddScoped<ForecastService>();

// Add MongoDB Seeder
builder.Services.AddScoped<MongoSeeder>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add API Explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Clothing POS API", 
        Version = "v1",
        Description = "A comprehensive Point of Sale API for clothing stores with MongoDB"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clothing POS API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at app's root
    });
}

// Enable CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Display startup information
Console.WriteLine("🚀 Clothing POS API Backend Started Successfully!");
Console.WriteLine("📍 API Server: http://localhost:5112");
Console.WriteLine("📖 Swagger Documentation: http://localhost:5112/swagger");
Console.WriteLine("🔧 Environment: " + app.Environment.EnvironmentName);
Console.WriteLine("=" + new string('=', 50));

// Test MongoDB connection and seed sample data (Optional)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        
        // Test MongoDB connection with timeout
        var database = context.Products.Database;
        await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(
            new MongoDB.Bson.BsonDocument("ping", 1), 
            cancellationToken: new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
        
        Console.WriteLine("✅ MongoDB connected successfully!");
        
        // Seed sample data
        var seeder = scope.ServiceProvider.GetRequiredService<MongoSeeder>();
        await seeder.SeedAsync();
        Console.WriteLine("✅ MongoDB seeded with sample data");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "⚠️ MongoDB connection failed, but API server is running");
        Console.WriteLine("⚠️ MongoDB connection failed, but API server is running on http://localhost:5112");
        Console.WriteLine("📝 You can test API endpoints with Swagger at: http://localhost:5112/swagger");
        Console.WriteLine("💡 MongoDB connection issue: SSL/TLS authentication problem");
    }
}

app.Run();
