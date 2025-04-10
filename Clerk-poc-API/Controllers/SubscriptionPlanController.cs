using Clerk_poc_API.Entities;
using Clerk_poc_API.Interfaces;
using Clerk_poc_API.Models;
using Clerk_poc_API.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Clerk_poc_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionPlanController : ControllerBase
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;

        public SubscriptionPlanController(ISubscriptionPlanService subscriptionPlanService)
        {
            _subscriptionPlanService = subscriptionPlanService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetPlans([FromQuery] string customerId)
        {
            var plans = await _subscriptionPlanService.GetAllPlansAsync(customerId);
            return Ok(plans);
        }
        [HttpPost("create-customer")]
        public async Task<IActionResult> Create([FromBody] StripeCustomerDto plan)
        {
            if (plan == null)
                return BadRequest("Plan is null.");

            var result = await _subscriptionPlanService.AddSubscriptionPlanAsync(plan);
            return Ok(result);
        }

        
    }
}
