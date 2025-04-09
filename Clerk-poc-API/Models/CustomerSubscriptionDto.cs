using Stripe;

namespace Clerk_poc_API.Models
{
    public class CustomerSubscriptionDto
    {
        public Customer Customer { get; set; }
        public Subscription Subscription { get; set; }
    }
}
