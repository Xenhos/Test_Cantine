using System.Net;
using System.Net.Http.Json;

public class CustomersTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateCustomer_ShouldReturnCustomer_WhenValidRequest()
    {
        var newCustomer = new Customer
        {
            Name = "John Doe",
            Type = CustomerType.Internal,
            Balance = 20.0m
        };

        var response = await Client.PostAsJsonAsync("/api/customers", newCustomer);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdCustomer = await response.Content.ReadFromJsonAsync<Customer>();
        Assert.NotNull(createdCustomer);
        Assert.Equal("John Doe", createdCustomer.Name);
        Assert.Equal(CustomerType.Internal, createdCustomer.Type);
        Assert.Equal(20.0m, createdCustomer.Balance);
    }

    [Fact]
    public async Task GetCustomer_ShouldReturnCustomer_WhenCustomerExists()
    {
        var newCustomer = new Customer
        {
            Name = "John Doe",
            Type = CustomerType.Contractor,
            Balance = 15.0m
        };
        await Context.Customers.InsertOneAsync(newCustomer);

        var response = await Client.GetAsync($"/api/customers/{newCustomer.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var retrievedCustomer = await response.Content.ReadFromJsonAsync<Customer>();
        Assert.NotNull(retrievedCustomer);
        Assert.Equal(newCustomer.Id, retrievedCustomer.Id);
        Assert.Equal(newCustomer.Name, retrievedCustomer.Name);
        Assert.Equal(newCustomer.Type, retrievedCustomer.Type);
        Assert.Equal(newCustomer.Balance, retrievedCustomer.Balance);
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnBadRequest_WhenInvalidData()
    {
        var invalidNameCustomer = new Customer
        {
            Name = "",
            Type = CustomerType.Internal,
            Balance = 20.0m
        };

        var invalidTypeCustomer = new Customer
        {
            Name = "John Doe",
            Type = (CustomerType)10,
            Balance = 20.0m
        };

        var invalidBalanceCustomer = new Customer
        {
            Name = "Jane Doe",
            Type = CustomerType.Internal,
            Balance = -5.0m
        };

        var nameResponse = await Client.PostAsJsonAsync("/api/customers", invalidNameCustomer);
        Assert.Equal(HttpStatusCode.BadRequest, nameResponse.StatusCode);

        var typeResponse = await Client.PostAsJsonAsync("/api/customers", invalidTypeCustomer);
        Assert.Equal(HttpStatusCode.BadRequest, typeResponse.StatusCode);

        var balanceResponse = await Client.PostAsJsonAsync("/api/customers", invalidBalanceCustomer);
        Assert.Equal(HttpStatusCode.BadRequest, balanceResponse.StatusCode);
    }

    [Fact]
    public async Task GetCustomer_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        var response = await Client.GetAsync("/api/customers/invalidid");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
