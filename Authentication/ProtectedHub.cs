using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Authentication
{
    [Authorize(AuthenticationSchemes = Schemes.CustomCookieScheme + "," + Schemes.CustomTokenScheme)]
    public class ProtectedHub : Hub
    {
        [Authorize("Cookie")]
        public object CookieProtected()
        {
            return CompileResult();
        }

        [Authorize("Token")]
        public object TokenProtected()
        {
            return CompileResult();
        }

        private object CompileResult() =>
            new
            {
                UserId = Context.UserIdentifier,
                Claims = Context.User.Claims.Select(x => new { x.Type, x.Value })
            };
    }
}
