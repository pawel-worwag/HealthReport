using System.Collections.Generic;
using System.Threading.Tasks;
using HealthReport.Identity.Application;
using HealthReport.Identity.Contracts.UserDetails;
using HealthReport.Identity.Contracts.UsersList;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HealthReport.Web.Components.Pages.Identity;

public partial class Users(IUsersListHandler usersListHandler,IUserDetailsHandler detailsHandler,
    IRolesListHandler rolesHandler, IJSRuntime JS) : ComponentBase
{
    [Inject]
    public IUsersListHandler UsersListHandler { get; set; } = usersListHandler;

    private ICollection<UserDto>? _users;
    private ICollection<string>? _allRoles;
    private bool _loading = true;

    private IJSObjectReference? _module;
    private UserDetailsDto? CurrentUser { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        _users = await UsersListHandler.ListAsync();
        _allRoles = await rolesHandler.ListAsync();
        _loading = false;
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await JS.InvokeAsync<IJSObjectReference>("import", "/Components/Pages/Identity/Users.razor.js");
            await _module.InvokeVoidAsync("initialize");
        }
    }
    
    private async Task ShowUserDetails(Guid id)
    {
        CurrentUser = await detailsHandler.GetByIdAsync(id);
        StateHasChanged();
        
        if (_module is not null)
        {
            await _module.InvokeVoidAsync("showModal");
        }
        else
        {
            Console.WriteLine("Module is null");
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
    }
}
