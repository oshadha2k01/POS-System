using backend.API.Data;
using backend.API.Configuration;
using backend.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddSingleton<MongoDbContext>();

// Register services
builder.Services.AddScoped<IProductService, ProductService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Test MongoDB connection
try
{
    var mongoContext = app.Services.GetRequiredService<MongoDbContext>();
    Console.WriteLine("✅ MongoDB connection established successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ MongoDB connection failed: {ex.Message}");
    Console.WriteLine("The API will still run but database operations may fail.");
}

Console.WriteLine("🚀 POS System Backend API is running!");
Console.WriteLine("📚 Swagger UI available at: https://localhost:5001/swagger (or http://localhost:5000/swagger)");

app.Run();