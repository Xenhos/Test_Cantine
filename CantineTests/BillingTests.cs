using System.Net;
using System.Net.Http.Json;

public class BillingTests : IntegrationTestBase
{
    [Fact]
    public async Task PayForMeal_ShouldReturnReceipt_WhenCustomerHasEnoughBalance()
    {
        var customer = new Customer
        {
            Name = "John Doe",
            Type = CustomerType.Internal,
            Balance = 20.0m
        };

        await Context.Customers.InsertOneAsync(customer);

        var mealTray = new MealTray
        {
            Products = new()
            {
                new Product { Name = "Salade", Type = ProductType.Starter, Price = 3.0m },
                new Product { Name = "Poulet", Type = ProductType.MainCourse, Price = 6.0m },
                new Product { Name = "Mousse au chocolat", Type = ProductType.Dessert, Price = 3.0m },
                new Product { Name = "Baguette", Type = ProductType.Bread, Price = 0.4m }
            }
        };

        var response = await Client.PostAsJsonAsync($"/api/billing/pay/{customer.Id}", mealTray);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var receipt = await response.Content.ReadFromJsonAsync<Receipt>();
        Assert.NotNull(receipt);
        Assert.Equal(2.50m, receipt.Total);

        var cResponse = await Client.GetAsync($"/api/customers/{customer.Id}");
        var retrievedCustomer = await cResponse.Content.ReadFromJsonAsync<Customer>();
        Assert.NotNull(retrievedCustomer);
        Assert.Equal(17.50m, retrievedCustomer.Balance);
    }

    [Fact]
    public async Task PayForMeal_ShouldReturnBadRequest_WhenCustomerHasInsufficientBalance()
    {
        var newCustomer = new Customer
        {
            Name = "Jane Doe",
            Type = CustomerType.Visitor,
            Balance = 2.0m
        };
        await Context.Customers.InsertOneAsync(newCustomer);

        var mealTray = new MealTray
        {
            Products = new()
            {
                new Product { Name = "Poulet", Type = ProductType.MainCourse, Price = 6.0m }
            }
        };

        var response = await Client.PostAsJsonAsync($"/api/billing/pay/{newCustomer.Id}", mealTray);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
