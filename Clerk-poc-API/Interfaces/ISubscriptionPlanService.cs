using Clerk_poc_API.Entities;
using Clerk_poc_API.Models;

namespace Clerk_poc_API.Interfaces
{
    public interface ISubscriptionPlanService
    {
        Task<List<SubscriptionPlanDto>> GetAllPlansAsync(string organizationId);
        Task<CustomerSubscriptionDto> AddSubscriptionPlanAsync(StripeCustomerDto plan);
        Task<SubscriptionStatusResult> IsSubscriptionActiveAsync(string organizationId);
    }
}
