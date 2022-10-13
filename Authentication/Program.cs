using Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication()
               .AddScheme<AuthenticationSchemeOptions, CustomCookie>(Schemes.CustomCookieScheme, _ =>
               {
               })
               .AddJwtBearer(Schemes.CustomTokenScheme, o =>
               {
                   o.Events = new()
                   {
                       OnMessageReceived = (context) =>
                       {
                           var path = context.HttpContext.Request.Path;
                           if (path.StartsWithSegments("/protected")
                               || path.StartsWithSegments("/token"))
                           {
                               var accessToken = context.Request.Query["access_token"];

                               if (!string.IsNullOrWhiteSpace(accessToken))
                               {
                                   // context.Token = accessToken;

                                   var claims = new Claim[]
                                   {
                                        new("user_id", accessToken),
                                        new("token", "token_claim"),
                                   };
                                   var identity = new ClaimsIdentity(claims, Schemes.CustomTokenScheme);
                                   context.Principal = new(identity);
                                   context.Success();
                               }
                           }

                           return Task.CompletedTask;
                       },
                   };
               }); ;

services.AddAuthorization(c =>
{
    c.AddPolicy("Cookie", pb => pb
        .AddAuthenticationSchemes(Schemes.CustomCookieScheme)
        .RequireAuthenticatedUser());

    c.AddPolicy("Token", pb => pb
        // schema get's ignored in signalr
        .AddAuthenticationSchemes(Schemes.CustomTokenScheme)
        .RequireClaim("token")
        .RequireAuthenticatedUser());
});

services.AddSignalR();

services.AddSingleton<IUserIdProvider, UserIdProvider>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ProtectedHub>("/protected", o =>
    {
        o.Transports = HttpTransportType.LongPolling;
    });

    endpoints.Map("/get-cookie", ctx =>
    {
        ctx.Response.StatusCode = 200;
        ctx.Response.Cookies.Append("signalr-auth-cookie", Guid.NewGuid().ToString(), new()
        {
            Expires = DateTimeOffset.UtcNow.AddSeconds(30)
        });
        return ctx.Response.WriteAsync("");
    });

    endpoints.Map("/token", ctx =>
    {
        ctx.Response.StatusCode = 200;
        return ctx.Response.WriteAsync(ctx.User?.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value);
    }).RequireAuthorization("Token");

    endpoints.Map("/cookie", ctx =>
    {
        ctx.Response.StatusCode = 200;
        return ctx.Response.WriteAsync(ctx.User?.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value);
    }).RequireAuthorization("Cookie");
});

app.Run();
