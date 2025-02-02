using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        try
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);

            //Console.WriteLine("Connected to MongoDB Atlas successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MongoDB connection error: {ex.Message}");
            
            throw;
        }
    }

    public IMongoCollection<Customer> Customers => _database.GetCollection<Customer>("Customers");
    public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
    public IMongoCollection<Receipt> Receipts => _database.GetCollection<Receipt>("Receipts");
}

public class MongoDbSettings
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
}
