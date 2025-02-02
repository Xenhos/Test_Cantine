using Microsoft.AspNetCore.Mvc;

[Route("api/billing")]
[ApiController]
public class BillingController : ControllerBase
{
    private readonly BillingService _billingService;

    public BillingController(BillingService billingService)
    {
        _billingService = billingService;
    }

    [HttpPost("pay/{customerId}")]
    public async Task<IActionResult> PayForMeal(string customerId, [FromBody] MealTray mealTray)
    {
        if (mealTray == null || mealTray.Products.Count == 0)
        {
            return BadRequest(new { message = "MealTray cannot be empty." });
        }

        foreach (var product in mealTray.Products)
        {
            product.Id = null;
        }

        try
        {
            var receipt = await _billingService.ProcessPayment(customerId, mealTray);

            return Ok(receipt);
        }
        catch (InsufficientBalanceException)
        {
            return BadRequest(new { message = "Insufficient balance to process payment." });
        }
        catch (CustomerNotFoundException)
        {
            return NotFound(new { message = "Customer not found." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while processing payment.", error = ex.Message });
        }
    }
}
