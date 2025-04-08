using Stripe;

namespace Clerk_poc_API.Services
{
    public class StripeService
    {
        private readonly string _freePlanPriceId;

        public StripeService(IConfiguration config)
        {
            StripeConfiguration.ApiKey = config["Stripe:ApiKey"];
            _freePlanPriceId = config["Stripe:FreePlanPriceId"];
        }

        public async Task<Subscription> CreateFreeSubscriptionAsync(string organizationName, string email)
        {
            // 1. Create Customer
            var customerOptions = new CustomerCreateOptions
            {
                Name = organizationName,
                Email = email
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
                TrialPeriodDays = 0
            };

            var subscriptionService = new SubscriptionService();
            return await subscriptionService.CreateAsync(subscriptionOptions);
        }
    }
}