using Clerk_poc_API.Interfaces;
using Clerk_poc_API.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetPlans([FromQuery] string tenantId)
        {
            var plans = await _subscriptionPlanService.GetAllPlansAsync(tenantId);
            return Ok(plans);
        }
    }
}
