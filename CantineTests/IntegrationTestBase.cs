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
        string mongoConnectionString = "mongodb+srv://vincentyvert:OwWe4hOx2JTAv0aw@cluster0.12bkz.mongodb.net/CantineKataDB?retryWrites=true&w=majority&appName=Cluster0";

        _mongoClient = new MongoClient(mongoConnectionString);
        var database = _mongoClient.GetDatabase("CantineKataDB");
        var settings = Options.Create(new MongoDbSettings
        {
            ConnectionString = mongoConnectionString,
            DatabaseName = "CantineKataDB"
        });

        Context = new MongoDbContext(settings);
         _database = _mongoClient.GetDatabase("CantineKataDB");
        //Client = new HttpClient { BaseAddress = new Uri("http://localhost:3001") };

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
    using (var collectionsCursor = await _database.ListCollectionNamesAsync())
    {
        var collections = await collectionsCursor.ToListAsync();
        foreach (var collection in collections)
        {
            await _database.DropCollectionAsync(collection);
        }
    }
}


    public Task DisposeAsync()
    {
        //_mongoClient.DropDatabase("CantineTestDB");
        _factory.Dispose();
        return Task.CompletedTask;
    }
}
