using Clerk_poc_API.Entities;
using Clerk_poc_API.Models;

namespace Clerk_poc_API.Interfaces
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionPlanDto>> GetAllPlansAsync(string tenantId);
    }
}
