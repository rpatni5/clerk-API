using Clerk.BackendAPI.Models.Operations;
using Clerk_poc_API.Interfaces;
using Clerk_poc_API.Services;
using Microsoft.AspNetCore.Mvc;

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
    }
}
