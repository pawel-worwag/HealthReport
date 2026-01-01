using Microsoft.AspNetCore.Components;

namespace HealthReport.Web.Components.Pages.Identity;

public partial class Login() : ComponentBase
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
}
