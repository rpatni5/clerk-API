using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Clerk_poc_API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/PaymentResult")]
    public class PaymentResultController : ControllerBase
    {
        [HttpGet("success")]
        public async Task<IActionResult> PaymentSuccess()
        {
            var redirectUrl = $"http://localhost:4200/admin/success";
            return Redirect(redirectUrl);
        }

        [HttpGet("canceled")]
        public IActionResult PaymentCanceled()
        {
            var redirectUrl = $"http://localhost:4200/admin/failure";
            return Redirect(redirectUrl);
        }
    }
}
