using SignalRPractise;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapHub<CustomHub>("/custom");
    endpoints.MapHub<CustomHub>("/groups");
});
app.Run();
