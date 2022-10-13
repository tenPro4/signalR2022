using ChatApp;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddAuthentication("Cookie")
                .AddCookie("Cookie");

services.AddSingleton<ChatRegistry>();
services.AddSingleton<IUserIdProvider, UserIdProvider>();

services.AddSignalR();
services.AddControllers();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
    endpoints.MapDefaultControllerRoute();
});

app.Run();
