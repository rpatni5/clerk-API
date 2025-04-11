using Clerk.BackendAPI;
using Clerk.BackendAPI.Models.Components;
using Clerk.BackendAPI.Models.Operations;
using Clerk_poc_API.Entities;
using Clerk_poc_API.Interfaces;
using Clerk_poc_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Organization = Clerk.BackendAPI.Models.Components.Organization;

namespace Clerk_poc_API.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IConfiguration _configuration;
        private readonly ClerkBackendApi _clerkClient;
        private readonly List<Organization> _organizations = new List<Organization>();
        private readonly ClerkPocContext _context;

        public OrganizationService(IConfiguration configuration, ClerkPocContext context)
        {
            _configuration = configuration;
            _context = context;
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

        public async Task<ListOrganizationsResponse> ListOrganizationsAsync()
        {
            var request = new ListOrganizationsRequest();

            var response = await _clerkClient.Organizations.ListAsync(request);
            return response;
        }

        public async Task<Organization> GetOrganizationAsync(string organizationId)
        {
            var result = await _clerkClient.Organizations.GetAsync(
                organizationId: organizationId,
                includeMembersCount: false,
                includeMissingMemberWithElevatedPermissions: false
            );

            return result.Organization;
        }
        public async Task<bool> SaveOrganizationAsync(OrganizationDto org)
        {
            var existingOrg = await _context.Organization.FindAsync(org.Id);
            if (existingOrg != null)
            {
                throw new InvalidOperationException("Organization already exists.");
            }
            var entity = new Clerk_poc_API.Entities.Organization
            {
                Id = !string.IsNullOrEmpty(org.Id) ? org.Id : null,
                OrganizationName = org.OrganizationName,
                CreatedAt = org.CreatedAt,
                StripeCustomerId = org.StripeCustomerId
            };
            _context.Organization.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<OrganizationDto> UpdateOrganizationAsync(OrganizationDto org)
        {
            var existingOrg = await _context.Organization.FindAsync(org.Id);
            if (existingOrg == null)
            {
                throw new InvalidOperationException("Organization not found.");
            }

            existingOrg.StripeCustomerId = org.StripeCustomerId;
            _context.Organization.Update(existingOrg);
            await _context.SaveChangesAsync();

            return new OrganizationDto
            {
                Id = existingOrg.Id,
                OrganizationName = existingOrg.OrganizationName,
                CreatedAt = existingOrg.CreatedAt,
            };
        }
        public async Task<bool> MarkExpiredAsync(string organizationId)
        {
            var organizaiton = await _context.Organization
                .FirstOrDefaultAsync(p => p.Id == organizationId);

            if (organizaiton == null) return false;
            organizaiton.IsExpired = true;

            var activePlan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.OrganizationId == organizationId && p.IsActivated == true);

            if (activePlan != null)
            {
                if (!string.IsNullOrEmpty(activePlan.SubscriptionId))
                {
                    var subscriptionService = new SubscriptionService();
                    await subscriptionService.CancelAsync(activePlan.SubscriptionId);
                }

                activePlan.IsActivated = false;
            }

            await _context.SaveChangesAsync();

            return true;
        }

    }
}

