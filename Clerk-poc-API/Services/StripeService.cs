using Clerk_poc_API.Entities;
using Clerk_poc_API.Interfaces;
using Clerk_poc_API.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Clerk_poc_API.Services
{
    public class StripeService : IStripeService
    {
        private readonly string _freePlanPriceId;
        private readonly IOrganizationService _organizationService;
        public StripeService(IConfiguration config, IOrganizationService organizationService)
        {
            _organizationService = organizationService;
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
            _freePlanPriceId = "price_1RBDfALWKuD5pPy8LaNRHlOz";
        }

        public async Task<CustomerSubscriptionDto> CreateCustomerWithFreeSubs(StripeCustomerDto model)
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
            var organization = new OrganizationDto
            {
                StripeCustomerId = customer.Id,
                Id = model.OrganizationId,
                OrganizationName = model.OrganizationName,
                CreatedAt = model.OrganizationCreatedAt,
            };
            var resp = await _organizationService.SaveOrganizationAsync(organization);


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
                TrialPeriodDays = 14,
                CancelAt = DateTime.UtcNow.AddMinutes(5),
            };

            var subscriptionService = new SubscriptionService();
            var subscription = await subscriptionService.CreateAsync(subscriptionOptions);

            // 3. Return both Customer and Subscription
            return new CustomerSubscriptionDto
            {
                Customer = customer,
                Subscription = subscription
            };
        }

        public async Task<List<Product>> GetAllStripeProductsAsync()
        {
            var productService = new ProductService();

            var options = new ProductListOptions
            {
                Limit = 100,
                Active = true
            };

            var products = await productService.ListAsync(options);
            return products.ToList();
        }

        public async Task<List<(Product product, List<Price> prices)>> GetAllProductsWithPricesAsync()
        {
            var productService = new ProductService();
            var priceService = new PriceService();

            var products = await productService.ListAsync(new ProductListOptions
            {
                Limit = 100,
                Active = true,
            });

            var result = new List<(Product, List<Price>)>();

            foreach (var product in products)
            {
                var prices = await priceService.ListAsync(new PriceListOptions
                {
                    Product = product.Id,
                    Active = true,
                });

                result.Add((product, prices.ToList()));
            }

            return result;
        }

        public async Task<Subscription?> GetActiveSubscriptionAsync(string customerId)
        {
            var subscriptionService = new SubscriptionService();

            var options = new SubscriptionListOptions
            {
                Customer = customerId,
                Status = "active", // Optional, returns only active subscriptions
                Limit = 1 // Only need the latest one usually
            };

            var subscriptions = await subscriptionService.ListAsync(options);

            // Return the first active subscription, if any
            return subscriptions.FirstOrDefault();
        }

        public async Task<Session> CreateCheckoutSessionAsync(CheckoutRequestDto model)
        {
            var subscriptionService = new Stripe.SubscriptionService();

            // 1. Cancel any existing active subscriptions for the customer
            var existingSubscriptions = await subscriptionService.ListAsync(new SubscriptionListOptions
            {
                Customer = model.StripeCustomerId,
                Status = "active",
                Limit = 1
            });

            var currentSubscription = existingSubscriptions.FirstOrDefault();
            if (currentSubscription != null)
            {
                // Cancel immediately without prorating (you can tweak this behavior)
                await subscriptionService.CancelAsync(currentSubscription.Id, new SubscriptionCancelOptions
                {
                    InvoiceNow = true,
                    Prorate = false
                });
            }

            var options = new SessionCreateOptions
            {
                SuccessUrl = "https://localhost:7063/api/PaymentResult/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://localhost:7063/api/PaymentResult/canceled",
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price =model.PriceId,
                    Quantity = model.Quantity,
                }
            },
                Mode = "subscription",
                Customer = model.StripeCustomerId,
                Metadata = new Dictionary<string, string>
                {
                   { "OrganizationId", model.OrganizationId }
                }
            };
            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session;
        }
    }
}