using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HealthReport.Web.Components.Pages.Identity;

public partial class Login(NavigationManager navigation, AuthenticationStateProvider authStateProvider) : ComponentBase
{
    [Parameter] 
    [SupplyParameterFromQuery(Name = "error")]
    public int Error { get; set; } = 0;

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
    }
}
