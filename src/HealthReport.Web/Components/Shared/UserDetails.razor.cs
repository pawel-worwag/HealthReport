using HealthReport.Identity.Application;
using HealthReport.Identity.Contracts.UserDetails;
using Microsoft.AspNetCore.Components;


namespace HealthReport.Web.Components.Shared;

public partial class UserDetails(IUserDetailsHandler loadHandler)
    : ComponentBase
{
    [Parameter]
    public UserDetailsDto? User { get; set; }
/*
    private void ToggleRole(string role, object? value)
    {
        if (value is bool isChecked)
        {
            if (isChecked)
            {
                _assignedRoles.Add(role);
            }
            else
            {
                _assignedRoles.Remove(role);
            }
        }
    }
    */
}