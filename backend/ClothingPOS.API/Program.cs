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

// Add HttpClient for AI Model integration
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

// Seed MongoDB with sample data
using (var scope = app.Services.CreateScope())
{
    try
    {
        var seeder = scope.ServiceProvider.GetRequiredService<MongoSeeder>();
        await seeder.SeedAsync();
        Console.WriteLine("✅ MongoDB seeded with sample data");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ An error occurred while seeding MongoDB");
    }
}

app.Run();
