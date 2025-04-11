using Clerk.BackendAPI.Models.Operations;
using Clerk_poc_API.Interfaces;
using Clerk_poc_API.Models;
using Clerk_poc_API.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Clerk_poc_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateOrganizationRequestBody request)
        {
            var response = await _organizationService.CreateOrganizationAsync(request);

            return Ok(response);
        }

        [HttpGet("get")]
        public async Task<IActionResult> ListOrganizations()
        {
            var organizations = await _organizationService.ListOrganizationsAsync();
            return Ok(organizations);
        }

        [HttpGet("{organizationId}")]
        public async Task<IActionResult> GetOrganization(string organizationId)
        {
            var organization = await _organizationService.GetOrganizationAsync(organizationId);
            return Ok(organization);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveOrganization([FromBody] OrganizationDto organization)
        {
            if (organization == null)
                return BadRequest("Organization is null");

            var result = await _organizationService.SaveOrganizationAsync(organization);
            return Ok(result);
        }

        [HttpPost("mark-expire")]
        public async Task<IActionResult> MarkAsExpired([FromQuery] string organizationId)
        {
            var success = await _organizationService.MarkExpiredAsync(organizationId);
            if (success)
                return Ok(new { message = "Plan marked as expired." });

            return NotFound(new { message = "No active plan found for this organization." });
        }
    }
}
