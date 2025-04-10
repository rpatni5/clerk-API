namespace Clerk_poc_API.Models
{
    public class CheckoutRequestDto
    {
        public string? PriceId { get; set; }
        public int? Quantity { get; set; }
        public string? Mode { get; set; }
        public string? StripeCustomerId { get; set; }
        public string? OrganizationId { get; set; }
        public string? CurrentStripeSubscriptionId { get; set; }
    }
}
