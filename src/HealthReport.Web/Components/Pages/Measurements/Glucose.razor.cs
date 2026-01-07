using System.Threading.Tasks;
using System.Security.Claims;
using HealthReport.Application.Contracts.Measurements;
using HealthReport.Application.Handlers.Measurements;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HealthReport.Web.Components.Pages.Measurements;

public partial class Glucose(IBloodGlucoseMeasurementsHandler loadHandler, AuthenticationStateProvider authStateProvider) : ComponentBase
{
	// Logged-in user id (nullable if anonymous)
	public Guid? CurrentUserId { get; private set; }

	// Date range filter - inclusive
	public DateOnly From { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
	public DateOnly To { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
	
	private ICollection<BloodGlucoseMeasurementDto>? _data = null;

	protected override async Task OnInitializedAsync()
	{
		var state = await authStateProvider.GetAuthenticationStateAsync();
		var user = state.User;
		if (user?.Identity?.IsAuthenticated == true)
		{
			var id = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
			if (Guid.TryParse(id, out var gid))
				CurrentUserId = gid;
		}
	}

	// Apply handler invoked by the "Go" button. Implement loading logic here.
	private async Task Apply()
	{
		if (CurrentUserId is not null)
		{
			_data = await loadHandler.GetForPatientAsync((Guid)CurrentUserId, From, To);
		}
		else
		{
			await Console.Error.WriteLineAsync("No user id found!");
		}
		StateHasChanged();
	}
}