using Clerk.BackendAPI;
using Clerk.BackendAPI.Models.Components;
using Clerk.BackendAPI.Models.Operations;
using Clerk_poc_API.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clerk_poc_API.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IConfiguration _configuration;
        private readonly ClerkBackendApi _clerkClient;
        public OrganizationService(IConfiguration configuration)
        {
            _configuration = configuration;

            var token = _configuration["Clerk:BearerToken"];
            _clerkClient = new ClerkBackendApi(bearerAuth: token);
        }

        public async Task<CreateOrganizationResponse> CreateOrganizationAsync(
           CreateOrganizationRequestBody request)
        {


            var res = new CreateOrganizationRequestBody
            {

                Name = request.Name,
                Slug = request.Slug,
                MaxAllowedMemberships = request.MaxAllowedMemberships,
                PublicMetadata = request.PublicMetadata ?? new Dictionary<string, object>(),
                PrivateMetadata = request.PrivateMetadata ?? new Dictionary<string, object>()
            };

            try
            {
                var response = await _clerkClient.Organizations.CreateAsync(res);
                return response;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create organization", ex);
            }
        }

        public async Task<ListOrganizationsResponse>ListOrganizationsAsync()
        {
            var request = new ListOrganizationsRequest();

            var response = await _clerkClient.Organizations.ListAsync(request);
            return response;
        }
    }
}
