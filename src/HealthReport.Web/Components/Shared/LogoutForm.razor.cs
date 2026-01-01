using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HealthReport.Web.Components.Shared;

public partial class LogoutForm(NavigationManager navigation, 
    AuthenticationStateProvider authStateProvider,
    IHttpContextAccessor httpContextAccessor,
    IAntiforgery antiforgery) : ComponentBase
{
    private string? _csrfToken;
    
    protected override async Task OnInitializedAsync()
    {
            var ctx = httpContextAccessor.HttpContext;
            if (ctx != null)
            {
                var tokens = antiforgery.GetAndStoreTokens(ctx);
                _csrfToken = tokens.RequestToken;
            }
            await Task.CompletedTask;
    }
}