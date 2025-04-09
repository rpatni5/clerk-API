using Clerk.BackendAPI.Models.Operations;
using Clerk_poc_API.Interfaces;
using Clerk_poc_API.Models;
using Clerk_poc_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clerk_poc_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly IStripeService _stripeService;

        public StripeController(IStripeService stripeService)
        {
            _stripeService = stripeService;
        }

        [HttpPost("create-customer")]
        public async Task<IActionResult> CreateFreeTrialCustomer([FromBody] StripeCustomerDto dto)
        {
            var subscription = await _stripeService.CreateCustomerWithFreeSubs(dto);
            return Ok(subscription);
        }


        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _stripeService.GetAllStripeProductsAsync();
            return Ok(products);
        }

        [HttpGet("products-with-prices")]
        public async Task<IActionResult> GetProductsWithPrices()
        {
            var data = await _stripeService.GetAllProductsWithPricesAsync();

            var result = data.Select(p => new
            {
                Product = p.product,
                Prices = p.prices
            });

            return Ok(result);
        }


        [HttpGet("active-subscription/{customerId}")]
        public async Task<IActionResult> GetActiveSubscription(string customerId)
        {
            var subscription = await _stripeService.GetActiveSubscriptionAsync(customerId);

            if (subscription == null)
                return NotFound("No active subscription found for this customer.");

            return Ok(subscription);
        }
    }
}
