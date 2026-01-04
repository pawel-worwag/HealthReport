using Microsoft.AspNetCore.Components;

namespace HealthReport.Web.Components.Pages.Reports;

public partial class SimpleSvg : ComponentBase
{
    private int MinYear { get; } = 2000;
    private int MaxYear { get; } = DateTime.Now.Year;
    private int FromYear { get; set; }
    private int FromMonth { get; set; }
    private int ToYear { get; set; }
    private int ToMonth { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var nDate = DateTime.Now;
        ToYear = nDate.Year;
        ToMonth = nDate.Month;
        nDate=nDate.AddMonths(-4);
        FromYear = nDate.Year;
        FromMonth = nDate.Month;
    }
}