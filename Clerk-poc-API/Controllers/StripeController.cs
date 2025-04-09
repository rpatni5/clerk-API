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

        [HttpPost("assign-free-plan")]
        public async Task<IActionResult> AssignFreePlan([FromBody] StripeCustomerDto dto)
        {
            var subscription = await _stripeService.CreateFreeSubscriptionAsync(dto);
            return Ok(new
            {
                subscription.Id,
                subscription.Status
            });
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

    }
}
