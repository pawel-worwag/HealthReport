using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HealthReport.Web.Components.Pages.Auth;

using Microsoft.AspNetCore.Components;

public partial class Profile(AuthenticationStateProvider authStateProvider) : ComponentBase
{
    protected string? email;
    protected string? userId;
    protected string[]? roles;

    protected override async Task OnInitializedAsync()
    {
        var state = await authStateProvider.GetAuthenticationStateAsync();
        var user = state.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            email = user.FindFirst(c => c.Type == ClaimTypes.Email || c.Type == ClaimTypes.Name || c.Type == "email")?.Value;
            userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
            roles = user.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role").Select(c => c.Value).ToArray();
        }
    }

}
