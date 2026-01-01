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
            IAntiforgery antiforgery,
            UserManager<Domain.ApplicationUser> userManager,
            SignInManager<Domain.ApplicationUser> signInManager) =>
        {
            await antiforgery.ValidateRequestAsync(ctx); 
            var form = await ctx.Request.ReadFormAsync();
            var email = form["email"].FirstOrDefault();
            var password = form["password"].FirstOrDefault();
            var returnUrl = form["returnUrl"].FirstOrDefault() ?? "/identity/profile";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ctx.Response.Redirect("/identity/login?error=1");
                return;
            }

            var user = await userManager.FindByEmailAsync(email!);
            if (user == null)
            {
                ctx.Response.Redirect("/identity/login?error=1");
                return;
            }

            var ok = await userManager.CheckPasswordAsync(user, password!);
            if (!ok)
            {
                ctx.Response.Redirect("/identity/login?error=1");
                return;
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            ctx.Response.Redirect(returnUrl);
        });

        app.MapPost("/identity/logout",
            async (HttpContext ctx, SignInManager<Domain.ApplicationUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                ctx.Response.Redirect("/");
            });

        // Antiforgery token endpoint for client-side forms: sets cookie and returns request token
        app.MapGet("/antiforgery/token", (HttpContext ctx, Microsoft.AspNetCore.Antiforgery.IAntiforgery antiforgery) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(ctx);
            return Results.Json(new { token = tokens.RequestToken });
        });

        return app;
    }
}