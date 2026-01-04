using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HealthReport.Web.Components.Pages.Identity;

public partial class Login(NavigationManager navigation, 
    AuthenticationStateProvider authStateProvider,
    IHttpContextAccessor httpContextAccessor,
    IAntiforgery antiforgery) : ComponentBase
{
    private string? CsrfToken;
    
    [Parameter] 
    [SupplyParameterFromQuery(Name = "error")]
    public int Error { get; set; } = 0;
    
    
    [Parameter]
    [SupplyParameterFromQuery(Name = "ReturnUrl")]
    public string? ReturnUrl { get; set; }

    private string ErrorDescription
    {
        get
        {
            return Error switch
            {
                0 => "No error",
                1 => "Bad username or password",
                _ => "Unknown error"
            };
        }
    }

    protected override async Task OnInitializedAsync()
    {
        var auth = await authStateProvider.GetAuthenticationStateAsync();
        if (auth.User?.Identity?.IsAuthenticated == true)
        {
            navigation.NavigateTo("/identity/profile");
        }
        else
        {
            var ctx = httpContextAccessor.HttpContext;
            if (ctx != null)
            {
                var tokens = antiforgery.GetAndStoreTokens(ctx);
                CsrfToken = tokens.RequestToken;
            }
        }
    }
}
