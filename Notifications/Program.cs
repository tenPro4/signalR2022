using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using Notifications;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddAuthentication("Cookie")
                .AddCookie("Cookie");

services.AddSingleton<INotificationSink, NotificationService>();
services.AddHostedService(sp => (NotificationService)sp.GetService<INotificationSink>());
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
    endpoints.MapHub<NotificationHub>("/notificationHub");
    endpoints.MapDefaultControllerRoute();
});

app.Run();
