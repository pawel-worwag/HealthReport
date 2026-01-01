using HealthReport.Identity.Application;
using HealthReport.Identity.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace HealthReport.Web.Components.Shared;

public partial class UserDetails(UserManager<ApplicationUser> userManager, IRolesListHandler rolesListHandler)
    : ComponentBase
{
    [Parameter] public string? UserId { get; set; }

    private ApplicationUser? _user;
    private bool _loading = true;
    private ICollection<string>? _allRoles;
    private HashSet<string> _assignedRoles = new();

    private ICollection<string> _statusMessage = [];

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrWhiteSpace(UserId))
        {
            _statusMessage.Add("User ID is required");
            return;
        }

        if (Guid.TryParse(UserId, out var guid))
        {
            _user = await userManager.FindByIdAsync(guid.ToString());

            if (_user is null)
            {
                _statusMessage.Add("User not found");
                return;
            }

            var roles = await userManager.GetRolesAsync(_user);
            _assignedRoles = new HashSet<string>(roles);
        }
        else
        {
            _statusMessage.Add("Bad UserId format (GUID expected)");
        }
    }

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
}