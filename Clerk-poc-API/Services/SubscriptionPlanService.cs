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

        public async Task<SubscriptionStatusResult> IsSubscriptionActiveAsync(string organizationId)
        {
            var activeSubscription = await _context.SubscriptionPlans.Include(x => x.Organization)
                .FirstOrDefaultAsync(x => x.OrganizationId == organizationId);


            if (activeSubscription == null)
            {
                return new SubscriptionStatusResult
                {
                    IsActive = false,
                    Message = "No subscription found in the database."
                };
            }
            if (activeSubscription.IsActivated == false)
            {
                return new SubscriptionStatusResult
                {
                    IsActive = false,
                    Message = "Subscription is not activated in the database."
                };
            }
            if (activeSubscription.ProductId == null)
            {
                return new SubscriptionStatusResult
                {
                    IsActive = false,
                    Message = "Subscription product ID is missing."
                };
            }

            if (!activeSubscription.ExpiryDate.HasValue || activeSubscription.ExpiryDate.Value.Date <= DateTime.UtcNow.Date)
            {
                return new SubscriptionStatusResult
                {
                    IsActive = false,
                    Message = "Subscription has expired."
                };
            }
            if (string.IsNullOrEmpty(activeSubscription.Organization.StripeCustomerId))
            {
                return new SubscriptionStatusResult
                {
                    IsActive = false,
                    Message = "Stripe customer ID not found."
                };
            }
            try
            {
                var subscriptionService = new SubscriptionService();
                var subscriptions = await subscriptionService.ListAsync(new SubscriptionListOptions
                {
                    Customer = activeSubscription.Organization.StripeCustomerId,
                    Status = "all",
                    Limit = 1
                });

                var stripeSub = subscriptions.FirstOrDefault();

                if (stripeSub == null)
                {
                    return new SubscriptionStatusResult
                    {
                        IsActive = false,
                        Message = "No subscription found on Stripe."
                    };
                }

                switch (stripeSub.Status)
                {
                    case "active":
                        return new SubscriptionStatusResult
                        {
                            IsActive = true,
                            Message = "Subscription is active."
                        };

                    case "trialing":
                        return new SubscriptionStatusResult
                        {
                            IsActive = true,
                            Message = "Subscription is in trial period."
                        };

                    case "past_due":
                        return new SubscriptionStatusResult
                        {
                            IsActive = false,
                            Message = "Payment failed. Subscription is past due."
                        };

                    case "incomplete":
                        return new SubscriptionStatusResult
                        {
                            IsActive = false,
                            Message = "Payment was not completed. Subscription is incomplete."
                        };

                    case "incomplete_expired":
                        return new SubscriptionStatusResult
                        {
                            IsActive = false,
                            Message = "Subscription expired due to incomplete payment."
                        };

                    case "unpaid":
                        return new SubscriptionStatusResult
                        {
                            IsActive = false,
                            Message = "Subscription is unpaid due to failed payment attempts."
                        };

                    case "canceled":
                        return new SubscriptionStatusResult
                        {
                            IsActive = false,
                            Message = "Subscription was canceled."
                        };

                    default:
                        return new SubscriptionStatusResult
                        {
                            IsActive = false,
                            Message = $"Subscription has unknown Stripe status: {stripeSub.Status}"
                        };
                }
            }
            catch (Exception ex)
            {
                return new SubscriptionStatusResult
                {
                    IsActive = false,
                    Message = $"Error checking Stripe subscription: {ex.Message}"
                };
            }
        }
    }
}


