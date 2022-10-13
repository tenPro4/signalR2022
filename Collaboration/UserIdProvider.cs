using Microsoft.AspNetCore.SignalR;

namespace Collaboration
{
    public class UserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
        }
    }
}
