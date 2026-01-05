using System.Security.Claims;
using HealthReport.Application.Contracts.Reports;
using HealthReport.Application.Handlers.Reports.Simple;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HealthReport.Web.Components.Pages.Reports;

public partial class Simple(ISimpleReportHandler handler,AuthenticationStateProvider authStateProvider) : ComponentBase
{
	private string? _userId;
	private int MinYear { get; } = 2000;
	private int MaxYear { get; } = DateTime.Now.Year;
	private int Year { get; set; }
	private int Month { get; set; }
	
	private SimpleReportDto? Report { get; set; }

	protected override async Task OnInitializedAsync()
	{
		var state = await authStateProvider.GetAuthenticationStateAsync();
		var user = state.User;
		if (user?.Identity?.IsAuthenticated == true)
		{
			_userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
		}
		
		Year = MaxYear;
		Month = DateTime.Now.Month;
	}

	private void Next()
	{
		DateOnly date = new(Year, Month, 1);
		date = date.AddMonths(1);
		Console.WriteLine($"Next {date}");
		Year = date.Year;
		Month = date.Month;
		StateHasChanged();
	}	
	private void Prev()
	{
		DateOnly date = new(Year, Month, 1);
		date = date.AddMonths(-1);
		Console.WriteLine($"Prev {date}");
		Year = date.Year;
		Month = date.Month;
		StateHasChanged();
	}
	
	private async Task Submit()
	{
		Console.WriteLine($"Submitting report for {Year}-{Month}");
		if (!string.IsNullOrEmpty(_userId))
		{
			Report = handler.GenerateMonthlyReport(Year, Month, Guid.Parse(_userId));
		}
	}
}