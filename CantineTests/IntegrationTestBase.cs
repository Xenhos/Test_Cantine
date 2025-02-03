using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Testing;

public class IntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly MongoDbContext Context;
    private readonly MongoClient _mongoClient;
    private readonly IMongoDatabase _database;
    private readonly WebApplicationFactory<Program> _factory;

    private static bool _databaseCleared = false;

    public IntegrationTestBase()
    {
        //Letting this in clear for the sake of the tests but in a real world scenario, this should be in a configuration file
        string mongoConnectionString = "mongodb+srv://testuser:64v1HwFHfEShuBOH@cluster0.12bkz.mongodb.net/CantineKataDB?retryWrites=true&w=majority&appName=Cluster0";

        _mongoClient = new MongoClient(mongoConnectionString);
        
        var settings = Options.Create(new MongoDbSettings
        {
            ConnectionString = mongoConnectionString,
            DatabaseName = "CantineKataDB"
        });

        Context = new MongoDbContext(settings);
        _database = _mongoClient.GetDatabase(settings.Value.DatabaseName);

        _factory = new WebApplicationFactory<Program>();
        Client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        if (!_databaseCleared)
        {
            await ClearDatabase();
            _databaseCleared = true;
        }
    }

    private async Task ClearDatabase()
    {
        using var collectionsCursor = await _database.ListCollectionNamesAsync();
        var collections = await collectionsCursor.ToListAsync();
        foreach (var collection in collections)
        {
            await _database.DropCollectionAsync(collection);
        }
    }


    public Task DisposeAsync()
    {
        _factory.Dispose();

        return Task.CompletedTask;
    }
}
