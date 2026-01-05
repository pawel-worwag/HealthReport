using Microsoft.AspNetCore.Components;

namespace HealthReport.Web.Components.Shared.Charts;

public partial class TimePointSeriesGraph : ComponentBase
{
    [Parameter] public int Width { get; set; } = 800;
    [Parameter] public int Height { get; set; } = 300;
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string YAxeTitle { get; set; } = string.Empty;
    [Parameter] public string XAxeTitle { get; set; } = string.Empty;
    [Parameter] public string? Class { get; set; }
    [Parameter] public int YMin { get; set; } = 60;
    [Parameter] public int YMax { get; set; } = 200;
    [Parameter] public int YStep { get; set; } = 20;
    

    private int ReverseVPosition(int y)
    {
        return Height - y;
    }
}