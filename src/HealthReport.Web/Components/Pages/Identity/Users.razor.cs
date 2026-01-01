using System.Collections.Generic;
using System.Threading.Tasks;
using HealthReport.Identity.Application;
using HealthReport.Identity.Contracts.UsersList;
using Microsoft.AspNetCore.Components;

namespace HealthReport.Web.Components.Pages.Identity;

public partial class Users(IUsersListHandler usersListHandler) : ComponentBase
{
    [Inject]
    public IUsersListHandler UsersListHandler { get; set; } = usersListHandler;

    private ICollection<UserDto>? _users;
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        _users = await UsersListHandler.ListAsync();
        _loading = false;
    }
}
