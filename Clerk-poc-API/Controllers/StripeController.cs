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
        private readonly StripeService _stripeService;

        public StripeController(StripeService stripeService)
        {
            _stripeService = stripeService;
        }

        [HttpPost("assign-free-plan")]
        public async Task<IActionResult> AssignFreePlan([FromBody] stripeModel dto)
        {
            var subscription = await _stripeService.CreateFreeSubscriptionAsync(dto.OrgName, dto.AdminEmail);
            return Ok(new
            {
                subscription.Id,
                subscription.Status
            });
        }
    } }
