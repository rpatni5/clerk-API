using Clerk_poc_API.Models;
using Stripe;

namespace Clerk_poc_API.Services
{
    public class StripeService
    {
        private readonly string _freePlanPriceId;

        public StripeService(IConfiguration config)
        {
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
            _freePlanPriceId = "price_1RBDfALWKuD5pPy8LaNRHlOz";
        }

        public async Task<Subscription> CreateFreeSubscriptionAsync(StripeCustomerDto model)
        {
            // 1. Create Customer
            var customerOptions = new CustomerCreateOptions
            {
                Name = model.UserName,
                Email = model.Email,
                Metadata = new Dictionary<string, string>
                {
                    { "UserId", model.UserId },
                    { "OrganizationId", model.OrganizationId },
                    { "OrganizationName", model.OrganizationName }
                }
            };
            var priceService = new PriceService();
            var customerService = new CustomerService();
            var customer = await customerService.CreateAsync(customerOptions);

            // 2. Create Subscription (Free plan)
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions>
            {
                new SubscriptionItemOptions
                {
                    Price = _freePlanPriceId
                }
            },
                TrialPeriodDays = 14
            };

            var subscriptionService = new SubscriptionService();
            return await subscriptionService.CreateAsync(subscriptionOptions);
        }
    }
}