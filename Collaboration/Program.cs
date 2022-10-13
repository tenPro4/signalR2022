using Collaboration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddAuthentication("MyScheme")
               .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>("MyScheme", o =>
               {
               });

services.AddSignalR();
services.AddSingleton<IUserIdProvider, UserIdProvider>();
services.AddSingleton<State>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SquareHub>("/square");
});

app.Run();
