using HealthReport.Application;
using HealthReport.Identity;
using HealthReport.Infrastructure;
using HealthReport.Web.Components;
using HealthReport.Web.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Register layers services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration);

// Authentication: require HttpContext accessor and cookie auth for Identity
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Identity.IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = Microsoft.AspNetCore.Identity.IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Identity.IdentityConstants.ApplicationScheme;
})
    .AddCookie(Microsoft.AspNetCore.Identity.IdentityConstants.ApplicationScheme, opts =>
    {
        opts.LoginPath = "/auth/login";
        opts.LogoutPath = "/auth/logout";
    });

builder.Services.AddScoped<Microsoft.AspNetCore.Identity.SignInManager<HealthReport.Identity.Domain.ApplicationUser>>();
// Supply AuthenticationState to Blazor components from the current HttpContext
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, HealthReport.Web.HttpContextAuthenticationStateProvider>();

// Health checks: readiness (DB) and liveness (basic)
builder.Services.AddHealthChecksServices();

builder.Services.AddOpenApi(options =>
{
    // Specify the OpenAPI version to use
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
});

// Configure Razor Components / Blazor Server
builder.Services.AddRazorComponents(options =>
        options.DetailedErrors = true)
    .AddInteractiveServerComponents();

// Show detailed circuit exceptions to help debug Blazor Server errors
builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(opts =>
{
    opts.DetailedErrors = true;
});

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

// Add authentication/authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map minimal API endpoints
app.MapEndpoint();
app.MapIdentityEndpoints();

// Health endpoints are mapped via extension for clarity
app.MapHealthChecksEndpoints();

// Map Blazor root component in interactive server render mode
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
