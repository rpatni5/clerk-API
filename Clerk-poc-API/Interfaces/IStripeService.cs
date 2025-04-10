using Clerk_poc_API.Entities;
using Clerk_poc_API.Models;
using Stripe;
using Stripe.Checkout;

namespace Clerk_poc_API.Interfaces
{
    public interface IStripeService
    {
        Task<CustomerSubscriptionDto> CreateCustomerWithFreeSubs(StripeCustomerDto model);

        Task<List<Product>> GetAllStripeProductsAsync();
        Task<List<(Product product, List<Price> prices)>> GetAllProductsWithPricesAsync();
        Task<Subscription?> GetActiveSubscriptionAsync(string customerId);

        Task<Session> CreateCheckoutSessionAsync(CheckoutRequestDto model);

    }
}
