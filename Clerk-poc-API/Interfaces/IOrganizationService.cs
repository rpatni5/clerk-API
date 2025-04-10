using Clerk.BackendAPI.Models.Components;
using Clerk.BackendAPI.Models.Operations;
using Clerk_poc_API.Models;

namespace Clerk_poc_API.Interfaces
{
    public interface IOrganizationService
    {
        Task<CreateOrganizationResponse> CreateOrganizationAsync(CreateOrganizationRequestBody request);
        Task<ListOrganizationsResponse> ListOrganizationsAsync();
        Task<Organization> GetOrganizationAsync(string organizationId);
        Task<bool> SaveOrganizationAsync(OrganizationDto org);
        Task<OrganizationDto> UpdateOrganizationAsync(OrganizationDto org);
    }
}
