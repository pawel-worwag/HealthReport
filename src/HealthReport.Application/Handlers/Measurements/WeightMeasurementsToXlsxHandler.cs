using ClosedXML.Excel;
using HealthReport.Application.Contracts.Measurements;

namespace HealthReport.Application.Handlers.Measurements;

public static class WeightMeasurementsToXlsxHandler
{
    public static byte[] Export(ICollection<WeightMeasurementDto> data)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Weight measurements");
        
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}