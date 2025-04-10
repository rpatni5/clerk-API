using Clerk_poc_API.Entities;
using Clerk_poc_API.Interfaces;
using Clerk_poc_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Clerk_poc_API.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ClerkPocContext _context;
        private readonly IOrganizationService _organizationService;
        private readonly IStripeService _stripeService;
        public SubscriptionPlanService(ClerkPocContext context, IOrganizationService organizationService, IStripeService stripeService)
        {
            _context = context;
            _organizationService = organizationService;
            _stripeService = stripeService;
        }
        public async Task<List<SubscriptionPlanDto>> GetAllPlansAsync(string organizationId)
        {
            var productPriceList = await _stripeService.GetAllProductsWithPricesAsync();
            var activeSubscription = await _context.SubscriptionPlans
                .Where(x => x.OrganizationId == organizationId).FirstOrDefaultAsync();
            var result = productPriceList.Select(tuple =>
            {
                var product = tuple.product;
                var price = tuple.prices.FirstOrDefault();
                bool isActive = false;

                if (activeSubscription != null &&
                    activeSubscription.ProductId == product.Id &&
                    activeSubscription.ExpiryDate.HasValue &&
                    activeSubscription.ExpiryDate.Value.Date > DateTime.UtcNow.Date)
                {
                    isActive = true;
                }

                return new SubscriptionPlanDto
                {
                    Name = product.Name,
                    Subtitle = product.Description,
                    Features = product.MarketingFeatures != null ? string.Join(", ", product.MarketingFeatures.Select(f => f.Name)) : string.Empty,
                    Price = price != null ? (price.UnitAmount.Value / 100.0M).ToString("F2") + " " + price.Currency.ToUpper() : "Free",
                    priceId = price.Id,
                    ProductId = product.Id,
                    ActivePlanId = activeSubscription.ProductId != null ? activeSubscription.ProductId : null,
                    ExpiryDate = activeSubscription.ExpiryDate,
                    IsActive = isActive
                };
            }).ToList();
            return result;
        }

        public async Task<CustomerSubscriptionDto> AddSubscriptionPlanAsync(StripeCustomerDto model)
        {
            var entity = await _stripeService.CreateCustomerWithFreeSubs(model);
            var savesubscription = new SubscriptionPlans
            {
                IsActivated = true,
                SubscriptionId = entity.Subscription.Id,
                OrganizationId = entity.Customer.Metadata.TryGetValue("OrganizationId", out var orgId) ? orgId : null,
                DefaultUsers = 1,
                ExtraUsers = 0,
                CreatedDate = entity.Subscription.TrialStart,
                ExpiryDate = entity.Subscription.TrialEnd,
                SubscriptionAmount = entity.Subscription.Items.Data[0].Price.UnitAmount.Value / 100.0M,
                ProductId = entity.Subscription.Items.Data[0].Price.ProductId,
            };
            await _context.SubscriptionPlans.AddAsync(savesubscription);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> IsSubscriptionActiveAsync(string organizationId)
        {
            var activeSubscription = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(x => x.OrganizationId == organizationId);

            return activeSubscription != null &&
                   activeSubscription.ProductId != null &&
                   activeSubscription.ExpiryDate.HasValue &&
                   activeSubscription.ExpiryDate.Value.Date > DateTime.UtcNow.Date;
        }

    }

}
