using Clerk_poc_API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clerk_poc_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetPlans([FromQuery] string tenantId)
        {
            var plans = await _subscriptionService.GetAllPlansAsync(tenantId);
            return Ok(plans);
        }
    }
}
