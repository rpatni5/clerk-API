using Clerk_poc_API.Entities;
using Clerk_poc_API.Models;

namespace Clerk_poc_API.Interfaces
{
    public interface ISubscriptionPlanService
    {
        Task<List<SubscriptionPlanDto>> GetAllPlansAsync(string customerId);
        Task<CustomerSubscriptionDto> AddSubscriptionPlanAsync(StripeCustomerDto plan);
        Task<bool> IsSubscriptionActiveAsync(string organizationId);
    }
}
