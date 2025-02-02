using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddSingleton<CustomerService>();

builder.Services.AddSingleton<BillingService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run("http://localhost:3001");

public partial class Program{}