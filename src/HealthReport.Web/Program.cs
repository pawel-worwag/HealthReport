using System.Text.Json;
using Microsoft.AspNetCore.Http;
using HealthReport.Application;
using HealthReport.Infrastructure;
using HealthReport.Web.Components;
using HealthReport.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddRazorComponents(options =>
        options.DetailedErrors = true)
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Global status page rewrites for non-API requests (re-execute pipeline to /not-found)
app.UseStatusCodePagesWithReExecute("/not-found");

// Middleware that, after the rest of the pipeline runs, converts 404s for API paths
// into a JSON response. Registering it after UseStatusCodePages ensures its
// post-processing runs before the StatusCodePages middleware's post-processing.
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404 && !context.Response.HasStarted && context.Request.Path.StartsWithSegments("/api"))
    {
        context.Response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(new { error = "Not found" });
        await context.Response.WriteAsync(payload);
    }
});

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseStaticFiles();

app.MapEndpoint();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
