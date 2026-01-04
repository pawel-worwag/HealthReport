using Microsoft.AspNetCore.Components;

namespace HealthReport.Web.Components.Shared;

public partial class PrimaryMenu (NavigationManager navigation) : ComponentBase
{
    private async Task NavigateToElement(string path)
    {
        await Task.CompletedTask;
        navigation.NavigateTo(path, true);
    }
}