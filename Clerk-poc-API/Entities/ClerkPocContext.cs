using Microsoft.EntityFrameworkCore;
using System;

namespace Clerk_poc_API.Entities
{
    public class ClerkPocContext: DbContext
    {
        public ClerkPocContext(DbContextOptions<ClerkPocContext> options) : base(options) { }

        public DbSet<SubscriptionPlans> SubscriptionPlans { get; set; }
        public DbSet<Organization> Organization { get; set; }
        public DbSet<SubscriptionHistory> SubscriptionHistory { get; set; }
    }
}
