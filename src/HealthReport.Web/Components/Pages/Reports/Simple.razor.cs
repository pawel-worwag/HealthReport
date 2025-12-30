using HealthReport.Application.Handlers.Reports.Simple;
using Microsoft.AspNetCore.Components;

namespace HealthReport.Web.Components.Pages.Reports;

public partial class Simple(ISimpleReportHandler handler) : ComponentBase
{
	private int MinYear { get; } = 2000;
	private int MaxYear { get; } = DateTime.Now.Year;
	private int Year { get; set; }
	private int Month { get; set; }
	
	private SimpleReportDto? Report { get; set; }

	protected override void OnInitialized()
	{
		Year = MaxYear;
		Month = DateTime.Now.Month;
	}

	private async Task Submit()
	{
		Console.WriteLine($"Submitting report for {Year}-{Month}");
		Report = handler.GenerateMonthlyReport(Year, Month);
	}
}