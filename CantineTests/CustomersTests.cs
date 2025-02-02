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
    }

    [Fact]
    public async Task GetCustomer_ShouldReturnCustomer_WhenCustomerExists()
    {
        var newCustomer = new Customer
        {
            Name = "Jane Doe",
            Type = CustomerType.Contractor,
            Balance = 15.0m
        };
        await Context.Customers.InsertOneAsync(newCustomer);

        var response = await Client.GetAsync($"/api/customers/{newCustomer.Id}");
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var retrievedCustomer = await response.Content.ReadFromJsonAsync<Customer>();
        Assert.NotNull(retrievedCustomer);
        Assert.Equal(newCustomer.Id, retrievedCustomer.Id);
        Assert.Equal(newCustomer.Name, retrievedCustomer.Name);
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnBadRequest_WhenInvalidData()
    {
        var invalidCustomer = new Customer
        {
            Name = "",
            Type = CustomerType.Internal,
            Balance = -5.0m
        };

        var response = await Client.PostAsJsonAsync("/api/customers", invalidCustomer);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCustomer_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        var response = await Client.GetAsync("/api/customers/invalid-id");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
