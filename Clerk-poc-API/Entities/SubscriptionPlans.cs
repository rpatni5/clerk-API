using System.ComponentModel.DataAnnotations;

namespace Clerk_poc_API.Entities
{
    public class SubscriptionPlans
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Subtitle { get; set; }
        public string Features { get; set; }  
        public string Price { get; set; }
        public string ButtonText { get; set; }
        public string CssClass { get; set; }
    }
}
