using MongoDB.Driver;

public class CustomerService
{
    private readonly IMongoCollection<Customer> _customers;

    public CustomerService(MongoDbContext context)
    {
        _customers = context.Customers;
    }

    public async Task<List<Customer>> GetCustomersAsync() => await _customers.Find(c => true).ToListAsync();

    public async Task<Customer> GetCustomerByIdAsync(string customerId)
    {
        var customer = await _customers.Find(c => c.Id == customerId).FirstOrDefaultAsync();
        if (customer == null)
            throw new CustomerNotFoundException();
        
        return customer;
    }
    public async Task<Customer> AddCustomerAsync(Customer customer)
    {
        if (customer == null)
            throw new CustomerNotFoundException();

        await _customers.InsertOneAsync(customer);
        
        return customer;
    }
}
