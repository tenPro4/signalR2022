using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Notifications
{
    public class DefaultController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("/auth")]
        public IActionResult Authenticate(string user_id)
        {
            var claims = new Claim[]
            {
                new("user_id", user_id),
            };

            var identity = new ClaimsIdentity(claims, "Cookie");
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync("Cookie", principal);
            return Ok();
        }

        [Authorize]
        [HttpGet("/notify")]
        public async Task<IActionResult> Notify(string user, string message,
            [FromServices] INotificationSink _notificationSink)
        {
            await _notificationSink.PushAsync(new(user, message));
            return Ok();
        }
    }
}
