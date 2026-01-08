using ClosedXML.Excel;
using HealthReport.Application.Contracts.Measurements;

namespace HealthReport.Application.Handlers.Measurements;

public static class BloodPressureMeasurementsToXlsxHandler
{
    public static byte[] Export(ICollection<BloodPressureMeasurementDto> data)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Blood pressure measurements");
        
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}