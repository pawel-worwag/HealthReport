using HealthReport.Identity.Application;
using HealthReport.Identity.Contracts;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace HealthReport.Identity.Extensions;

public static class Endpoints
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        // Account endpoints for cookie sign-in (must be full HTTP requests so Set-Cookie is written to browser)
        app.MapPost("/identity/login-submit", async (HttpContext ctx,
            IAntiforgery antiforgery, ILoginUserHandler handler) =>
        {
            await antiforgery.ValidateRequestAsync(ctx); 
            var form = await ctx.Request.ReadFormAsync();
            var email = form["email"].FirstOrDefault();
            var password = form["password"].FirstOrDefault();
            var returnUrl = form["returnUrl"].FirstOrDefault() ?? "/identity/profile";
            var result = await handler.LoginAsync(new LoginUserDto(email, password));
            if (!result.Succeeded)
            {
                ctx.Response.Redirect("/identity/login?error=1");
                return;
            }
            ctx.Response.Redirect(returnUrl);
        });

        app.MapPost("/identity/logout-submit",
            async (HttpContext ctx,
                IAntiforgery antiforgery, SignInManager<Domain.ApplicationUser> signInManager) =>
            {
                await antiforgery.ValidateRequestAsync(ctx); 
                await signInManager.SignOutAsync();
                ctx.Response.Redirect("/");
            });

        // Antiforgery token endpoint for client-side forms: sets cookie and returns request token
        app.MapGet("/antiforgery/token", (HttpContext ctx, IAntiforgery antiforgery) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(ctx);
            return Results.Json(new { token = tokens.RequestToken });
        });

        // Roles list endpoint: returns all available role names
        app.MapGet("/identity/roles", async (IRolesListHandler handler) =>
        {
            var roles = await handler.ListAsync();
            return Results.Json(roles);
        });

        return app;
    }
}