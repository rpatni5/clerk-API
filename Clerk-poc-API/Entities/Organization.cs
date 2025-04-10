namespace Clerk_poc_API.Entities
{
    public class Organization
    {
        public string? Id { get; set; } 
        public DateTime CreatedAt { get; set; }
        public string? OrganizationName { get; set; }
        public string? StripeCustomerId { get; set; }
    }

}
