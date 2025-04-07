using Clerk.BackendAPI.Models.Components;
using Clerk.BackendAPI.Models.Operations;

namespace Clerk_poc_API.Interfaces
{
    public interface IOrganizationService
    {
        Task<CreateOrganizationResponse> CreateOrganizationAsync(CreateOrganizationRequestBody request);
        Task<ListOrganizationsResponse> ListOrganizationsAsync();
    }
}
