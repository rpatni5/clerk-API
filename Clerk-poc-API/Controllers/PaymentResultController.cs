using Clerk_poc_API.Entities;
using Clerk_poc_API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Clerk_poc_API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/PaymentResult")]
    public class PaymentResultController : ControllerBase
    {

        private readonly ClerkPocContext _context;
        public PaymentResultController(ClerkPocContext context)
        {
            _context = context;
        }
        [HttpGet("success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string session_id)
        {
            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(session_id);

            var customerService = new CustomerService();
            var customer = await customerService.GetAsync(session.CustomerId);

            var subscriptionService = new SubscriptionService();
            var subscription = await subscriptionService.GetAsync(session.SubscriptionId);

            var organizationId = session.Metadata.TryGetValue("OrganizationId", out var orgId) ? orgId : null;

            if (organizationId != null)
            {
                //mark the organization as not expired
                var organization = await _context.Organization.FirstOrDefaultAsync(o => o.Id == organizationId);
                if (organization != null)
                {
                    organization.IsExpired = false;
                }
                // 1. Get the existing active plan for the organization
                var existingPlan = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(p => p.OrganizationId == organizationId);

                if (existingPlan != null)
                {
                    // 2. Archive the existing plan
                    var history = new SubscriptionHistory
                    {
                        OrganizationId = existingPlan.OrganizationId,
                        SubscriptionId = existingPlan.SubscriptionId,
                        DefaultUsers = existingPlan.DefaultUsers,
                        ExtraUsers = existingPlan.ExtraUsers,
                        CreatedDate = existingPlan.CreatedDate,
                        ExpiryDate = existingPlan.ExpiryDate,
                        SubscriptionAmount = existingPlan.SubscriptionAmount,
                        ProductId = existingPlan.ProductId,
                    };

                    _context.SubscriptionHistory.Add(history);

                    // 3. Update the same plan with new data
                    existingPlan.SubscriptionId = subscription.Id;
                    existingPlan.DefaultUsers = 1;
                    existingPlan.ExtraUsers = 0;
                    existingPlan.CreatedDate = subscription.Items.Data[0].CurrentPeriodStart;
                    existingPlan.ExpiryDate = subscription.Items.Data[0].CurrentPeriodEnd;
                    existingPlan.SubscriptionAmount = subscription.Items.Data[0].Price.UnitAmount.Value / 100.0M;
                    existingPlan.ProductId = subscription.Items.Data[0].Price.ProductId;
                    existingPlan.IsActivated = true; // in case it was false
                }
                else
                {
                    // If no existing plan found, create a new one
                    var newPlan = new SubscriptionPlans
                    {
                        IsActivated = true,
                        SubscriptionId = subscription.Id,
                        OrganizationId = organizationId,
                        DefaultUsers = 1,
                        ExtraUsers = 0,
                        CreatedDate = subscription.Items.Data[0].CurrentPeriodStart,
                        ExpiryDate = subscription.Items.Data[0].CurrentPeriodEnd,
                        SubscriptionAmount = subscription.Items.Data[0].Price.UnitAmount.Value / 100.0M,
                        ProductId = subscription.Items.Data[0].Price.ProductId
                    };

                    _context.SubscriptionPlans.Add(newPlan);
                }

                await _context.SaveChangesAsync();
            }

            return Redirect("http://localhost:4200/admin/success");
        }

        [HttpGet("canceled")]
        public IActionResult PaymentCanceled()
        {
            var redirectUrl = $"http://localhost:4200/admin/failure";
            return Redirect(redirectUrl);
        }
    }
}
