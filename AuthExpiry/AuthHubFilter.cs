using Microsoft.AspNetCore.SignalR;

namespace AuthExpiry
{
    public class AuthException : HubException
    {
        public AuthException(string message) : base(message)
        {
        }
    }

    public class AuthHubFilter : IHubFilter
    {
        public async ValueTask<object> InvokeMethodAsync(
           HubInvocationContext invocationContext,
           Func<HubInvocationContext, ValueTask<object>> next
       )
        {
            await Task.Delay(1000);
            var expiry = invocationContext.Context.User.Claims.FirstOrDefault(x => x.Type == "expires").Value;
            var expiryDate = new DateTimeOffset(long.Parse(expiry), TimeSpan.Zero);
            //var dddd = Convert.ToDateTime(expiry);
            //var matchFound = (expiryDate - DateTime.Now).TotalDays < 90;
            if (DateTimeOffset.UtcNow.Subtract(expiryDate) > TimeSpan.Zero)
            {
                throw new AuthException("auth_expired");
                // await invocationContext.Hub.Clients.Caller.SendAsync("session_expired");
            }

            return await next(invocationContext);
        }
        public Task OnConnectedAsync(
            HubLifetimeContext context,
            Func<HubLifetimeContext, Task> next
        )
        {
            return next(context);
        }

        public Task OnDisconnectedAsync(
            HubLifetimeContext context,
            Exception exception,
            Func<HubLifetimeContext, Exception, Task> next
        )
        {
            return next(context, exception);
        }
    }
}
