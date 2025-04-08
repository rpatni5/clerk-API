using Clerk_poc_API.Entities;
using Clerk_poc_API.Interfaces;
using Clerk_poc_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Clerk_poc_API.Services
{
    public class SubscriptionService :ISubscriptionService
    {
        private readonly ClerkPocContext _context;
        private readonly IOrganizationService _organizationService;
        public SubscriptionService(ClerkPocContext context, IOrganizationService organizationService)
        {
            _context = context;
            _organizationService = organizationService;
        }
        public async Task<List<SubscriptionPlanDto>> GetAllPlansAsync(string tenantId)
        {
            var CreatedAt = _organizationService.GetOrganizationAsync(tenantId).Result.CreatedAt;
            var plans = await _context.SubscriptionPlans.ToListAsync();
            var entities = plans.Select(plan => new SubscriptionPlanDto
            {
                Name = plan.Name,
                Subtitle = plan.Subtitle,
                Features = plan.Features,
                Price = plan.Price,
                ButtonText = plan.ButtonText,
                CssClass= plan.CssClass,
                CreatedAt = CreatedAt 
            }).ToList();
            return entities;
        }
    }
    
}
