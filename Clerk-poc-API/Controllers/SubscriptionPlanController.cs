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
        public async Task<IActionResult> GetPlans([FromQuery] string organizationId)
        {
            var plans = await _subscriptionPlanService.GetAllPlansAsync(organizationId);
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


        [HttpGet("check-status")]
        public async Task<IActionResult> CheckSubscriptionStatus([FromQuery] string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId))
            {
                return BadRequest("OrganizationId is required.");
            }

            var isActive = await _subscriptionPlanService.IsSubscriptionActiveAsync(organizationId);
            return Ok(isActive);
        }
    }
}
