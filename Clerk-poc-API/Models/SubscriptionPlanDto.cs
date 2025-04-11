namespace Clerk_poc_API.Models
{
    public class SubscriptionPlanDto
    {
        public string Name { get; set; }
        public string Subtitle { get; set; }
        public string Features { get; set; }
        public string Price { get; set; }
        public string CssClass { get; set; }
        public long CreatedAt { get; set; }
        public string? priceId { get; set; }
        public string ? ProductId { get; set; }
        public string?  ActivePlanId{get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? IsActivated { get; set; }
    }
}
