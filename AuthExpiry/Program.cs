using AuthExpiry;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddAuthentication()
               .AddScheme<AuthenticationSchemeOptions, CustomCookie>(Schemes.CustomCookieScheme, _ =>
               {
               });

services.AddSignalR(o =>
{
    o.AddFilter<AuthHubFilter>();
});

services.AddSingleton<IUserIdProvider, UserIdProvider>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ProtectedHub>("/protected");

    endpoints.Map("/get-cookie", ctx =>
    {
        ctx.Response.StatusCode = 200;
        ctx.Response.Cookies.Append("signalr-auth-cookie", Guid.NewGuid().ToString(), new()
        {
            Expires = DateTimeOffset.UtcNow.AddSeconds(30)
        });
        return ctx.Response.WriteAsync("");
    });
});

app.Run();
