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

// Register application-specific middleware
app.UseCustomMiddlewares();

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseStaticFiles();

app.MapEndpoint();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
