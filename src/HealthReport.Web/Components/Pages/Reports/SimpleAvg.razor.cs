using System.Security.Claims;
using HealthReport.Application.Contracts.Reports;
using HealthReport.Application.Handlers.Reports.SimpleAvg;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HealthReport.Web.Components.Pages.Reports;

public partial class SimpleAvg(ISimpleAvgReportHandler handler,AuthenticationStateProvider authStateProvider) : ComponentBase
{
    protected string? userId;
    private int MinYear { get; } = 2000;
    private int MaxYear { get; } = DateTime.Now.Year;
    private int FromYear { get; set; }
    private int FromMonth { get; set; }
    private int ToYear { get; set; }
    private int ToMonth { get; set; }
    
    private SimpleAvgReportDto? report = null;

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        var state = await authStateProvider.GetAuthenticationStateAsync();
        var user = state.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
        }
        
        var nDate = DateTime.Now;
        ToYear = nDate.Year;
        ToMonth = nDate.Month;
        nDate=nDate.AddMonths(-4);
        FromYear = nDate.Year;
        FromMonth = nDate.Month;
    }
    
    private async Task Submit()
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var from = new DateOnly(FromYear, FromMonth, 1);
            var to = new DateOnly(ToYear, ToMonth, DateTime.DaysInMonth(ToYear, ToMonth));
            
            report = await handler.GenerateReportAsync(from, to, Guid.Parse(userId));
        }
    }
}