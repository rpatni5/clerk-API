namespace Clerk_poc_API.Entities
{
    public class SubscriptionHistory
    {
        public int Id { get; set; }

        public string? OrganizationId { get; set; }

        public string? SubscriptionId { get; set; }

        public int? DefaultUsers { get; set; }

        public int? ExtraUsers { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public decimal? SubscriptionAmount { get; set; }

        public string? ProductId { get; set; }
    }

}
