using System.ComponentModel.DataAnnotations;

namespace Clerk_poc_API.Entities
{
    public class SubscriptionPlans
    {
        [Key]
        public int Id { get; set; }
        public bool? IsActivated { get; set; }
        public string? SubscriptionId { get; set; }
        public string? OrganizationId { get; set; }  
        public string? ProductId { get; set; }
        public int? DefaultUsers { get; set; }  
        public int? ExtraUsers { get; set; }
        public DateTime CreatedDate { get; set; } 
        public DateTime ExpiryDate { get; set; }
        public decimal SubscriptionAmount { get; set; }

    }
}
