using HealthReport.Application;
using HealthReport.Infrastructure;
using HealthReport.Web.Components;
using HealthReport.Web.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Register layers services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddOpenApi(options =>
{
    // Specify the OpenAPI version to use
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
});

// Configure Razor Components / Blazor Server
builder.Services.AddRazorComponents(options =>
        options.DetailedErrors = true)
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.MapOpenApi();
app.MapScalarApiReference("/docs");

// Re-execute pipeline to '/not-found' for non-API status pages (shows NotFound component)
// Note: keep this before static files/endpoints so that re-execute can render the Blazor component.
app.UseStatusCodePagesWithReExecute("/not-found");

// Register application-specific middleware
app.UseCustomMiddlewares();

app.UseHttpsRedirection();
// CSRF protection middleware; required for endpoints that expect antiforgery tokens
app.UseAntiforgery();

// Serve static files (wwwroot)
app.UseStaticFiles();

// Map minimal API endpoints
app.MapEndpoint();

// Map Blazor root component in interactive server render mode
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
