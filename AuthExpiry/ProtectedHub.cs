using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Net;

namespace AuthExpiry
{
    [Authorize(AuthenticationSchemes = Schemes.CustomCookieScheme)]
    public class ProtectedHub : Hub
    {
        public string AuthorizedResource()
        {
            return "authorized resource";
        }

        // no reason can be specified
        public void Abort() => Context.Abort();
    }
}
