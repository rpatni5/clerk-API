namespace Clerk_poc_API.Models
{
    public class StripeCustomerDto
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public DateTime OrganizationCreatedAt { get; set; }
    }
}
