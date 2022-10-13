using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AuthExpiry
{
    public class CustomCookie : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomCookie(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        ) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Context.Request.Cookies.TryGetValue("signalr-auth-cookie", out var cookie))
            {
                var claims = new Claim[]
                {
                        new("user_id", cookie),
                        new("cookie", "cookie_claim"),
                        new("expires", DateTimeOffset.UtcNow.AddSeconds(30).Ticks.ToString()),
                };
                var identity = new ClaimsIdentity(claims, Schemes.CustomCookieScheme);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, new(), Schemes.CustomCookieScheme);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail("signalr-auth-cookie not found"));
        }
    }

    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
        }
    }
}
