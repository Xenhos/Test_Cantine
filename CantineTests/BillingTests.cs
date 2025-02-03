using System.Net;
using System.Net.Http.Json;

public class BillingTests : IntegrationTestBase
{
    [Fact]
    public async Task PayForMeal_ShouldReturnReceipt_WhenCustomerHasEnoughBalance()
    {
        var internalCustomer = new Customer
        {
            Name = "John Doe",
            Type = CustomerType.Internal,
            Balance = 10.0m
        };

        await Context.Customers.InsertOneAsync(internalCustomer);

        var vipCustomer = new Customer
        {
            Name = "Jane Doe",
            Type = CustomerType.VIP,
            Balance = 5.0m
        };

        await Context.Customers.InsertOneAsync(vipCustomer);

        var mealTray = new MealTray
        {
            Products =
            [
                new Product { Name = "Salade", Type = ProductType.Starter, Price = 3.0m },
                new Product { Name = "Poulet", Type = ProductType.MainCourse, Price = 6.0m },
                new Product { Name = "Mousse au chocolat", Type = ProductType.Dessert, Price = 3.0m },
                new Product { Name = "Baguette", Type = ProductType.Bread, Price = 0.4m }
            ]
        };

        var response = await Client.PostAsJsonAsync($"/api/billing/pay/{internalCustomer.Id}", mealTray);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var receipt = await response.Content.ReadFromJsonAsync<Receipt>();
        Assert.NotNull(receipt);
        Assert.Equal(2.50m, receipt.Total);

        response = await Client.GetAsync($"/api/customers/{internalCustomer.Id}");
        var retrievedCustomer = await response.Content.ReadFromJsonAsync<Customer>();
        Assert.NotNull(retrievedCustomer);
        Assert.Equal(7.50m, retrievedCustomer.Balance);

        response = await Client.PostAsJsonAsync($"/api/billing/pay/{vipCustomer.Id}", mealTray);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        receipt = await response.Content.ReadFromJsonAsync<Receipt>();
        Assert.NotNull(receipt);
        Assert.Equal(0.0m, receipt.Total);

        response = await Client.GetAsync($"/api/customers/{vipCustomer.Id}");
        retrievedCustomer = await response.Content.ReadFromJsonAsync<Customer>();
        Assert.NotNull(retrievedCustomer);
        Assert.Equal(5.0m, retrievedCustomer.Balance);
    }

    [Fact]
    public async Task PayForMeal_ShouldReturnBadRequest_WhenCustomerHasInsufficientBalance()
    {
        var newCustomer = new Customer
        {
            Name = "John Doe",
            Type = CustomerType.Visitor,
            Balance = 2.0m
        };
        await Context.Customers.InsertOneAsync(newCustomer);

        var mealTray = new MealTray
        {
            Products =
            [
                new Product { Name = "Poulet", Type = ProductType.MainCourse, Price = 6.0m }
            ]
        };

        var response = await Client.PostAsJsonAsync($"/api/billing/pay/{newCustomer.Id}", mealTray);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
