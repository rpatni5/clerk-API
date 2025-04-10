namespace Clerk_poc_API.Models
{
    public class OrganizationDto
    {
        public string? Id { get; set; }
        public string? OrganizationName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? StripeCustomerId { get; set; }
    }
}
