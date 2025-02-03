using Microsoft.AspNetCore.Mvc;

[Route("api/customers")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _customerService.GetCustomersAsync();

        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerById(string id)
    {
        try
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            return Ok(customer);
        }
        catch (CustomerNotFoundException)
        {
            return NotFound(new { message = "Customer not found." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
    {
        if (!Enum.IsDefined(typeof(CustomerType), customer.Type))
            return BadRequest("Invalid customer type.");

        if (string.IsNullOrWhiteSpace(customer.Name))
            return BadRequest("Customer name is required.");

        if (customer.Balance < 0)
            return BadRequest("Balance cannot be negative.");

        try
        {
            var newCustomer = await _customerService.AddCustomerAsync(customer);
            
            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.Id }, newCustomer);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
