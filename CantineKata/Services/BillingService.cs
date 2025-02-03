using MongoDB.Driver;

public class BillingService
{
    private readonly IMongoCollection<Customer> _customers;
    private readonly IMongoCollection<Receipt> _receipts;

    public BillingService(MongoDbContext context)
    {
        _customers = context.Customers;
        _receipts = context.Receipts;
    }

    public async Task<Receipt?> ProcessPayment(string customerId, MealTray mealTray)
    {
        var customer = await _customers.Find(c => c.Id == customerId).FirstOrDefaultAsync();
        if (customer == null)
            throw new CustomerNotFoundException();

        foreach (var product in mealTray.Products)
        {
            product.Id = null;
        }

        decimal totalPrice = CalculateMealPrice(mealTray, customer.Type);
        if (customer.Balance < totalPrice && customer.Type != CustomerType.Internal && customer.Type != CustomerType.VIP)
            throw new InsufficientBalanceException();

        customer.Balance -= totalPrice;
        await _customers.ReplaceOneAsync(c => c.Id == customerId, customer);

        var receipt = new Receipt
        {
            CustomerId = customerId,
            Products = mealTray.Products,
            Total = totalPrice
        };

        await _receipts.InsertOneAsync(receipt);

        return receipt;
    }

    private decimal CalculateMealPrice(MealTray mealTray, CustomerType customerType)
    {
        decimal total = 0m;

        if (IsFixedMealPrice(mealTray))
            total = 10.0m;
        else
        {
            foreach (var product in mealTray.Products)
                total += product.Price;
        }

        total -= customerType switch
        {
            CustomerType.Internal => 7.5m,
            CustomerType.Contractor => 6m,
            CustomerType.VIP => total,
            CustomerType.Intern => 10m,
            _ => 0m
        };

        return total < 0 ? 0 : total;
    }

    private bool IsFixedMealPrice(MealTray mealTray)
    {
        return mealTray.Products.Count == 4 &&
               mealTray.Products.Exists(p => p.Type == ProductType.Starter) &&
               mealTray.Products.Exists(p => p.Type == ProductType.MainCourse) &&
               mealTray.Products.Exists(p => p.Type == ProductType.Dessert) &&
               mealTray.Products.Exists(p => p.Type == ProductType.Bread);
    }
}