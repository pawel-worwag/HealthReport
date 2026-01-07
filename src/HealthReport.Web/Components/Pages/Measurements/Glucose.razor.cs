using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace HealthReport.Web.Components.Pages.Measurements;

public partial class Glucose : ComponentBase
{
	// Date range filter - inclusive
	public DateOnly From { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
	public DateOnly To { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

	// Apply handler invoked by the "Go" button. Implement loading logic here.
	public async Task Apply()
	{
		// TODO: call service / load data based on From/To
		await Task.CompletedTask;
		StateHasChanged();
	}
}