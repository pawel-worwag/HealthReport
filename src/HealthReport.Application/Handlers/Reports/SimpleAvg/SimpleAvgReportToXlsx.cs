using ClosedXML.Excel;
using HealthReport.Application.Contracts.Reports;

namespace HealthReport.Application.Handlers.Reports.SimpleAvg;

public static class SimpleAvgReportToXlsx
{
    public static byte[] Export(SimpleAvgReportDto report)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Simple report");
        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
        ws.PageSetup.Margins.Top = 0.2;
        ws.PageSetup.Margins.Bottom = 0.2;
        ws.PageSetup.Margins.Left = 0.2;
        ws.PageSetup.Margins.Right = 0.2;
        ws.PageSetup.CenterHorizontally = true;
        ws.PageSetup.CenterVertically = false;

        ws.Cell(1, 1).Value = $"Date:";
        ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        ws.Range(1, 2, 1, 11).Merge().Value = $"from {report.DateFrom} to {report.DateTo}";
        var title = ws.Range(1, 1, 1, 19);
        title.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        title.Style.Border.BottomBorderColor = XLColor.FromArgb(0, 0, 0);


        var th1 = ws.Range(3, 1, 5, 1).Merge();
        th1.Value = "Month";
        th1.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        th1.Style.Border.OutsideBorderColor = XLColor.FromArgb(0, 0, 0);

        var th2 = ws.Range(3, 2, 3, 7).Merge();
        th2.Value = "Blood pressure [mmHg]";
        th2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        th2.Style.Border.OutsideBorderColor = XLColor.FromArgb(0, 0, 0);

        var th21 = ws.Range(4, 2, 4, 4).Merge();
        th21.Value = "Systolic";
        th21.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        th21.Style.Border.OutsideBorderColor = XLColor.FromArgb(0, 0, 0);

        var th22 = ws.Range(4, 5, 4, 7).Merge();
        th22.Value = "Diastolic";
        th22.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        th22.Style.Border.OutsideBorderColor = XLColor.FromArgb(0, 0, 0);
        
        

        var th3 = ws.Range(3, 8, 4, 10).Merge();
        th3.Value = "Pulse [bpm]";
        th3.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        th3.Style.Border.OutsideBorderColor = XLColor.FromArgb(0, 0, 0);

        var th4 = ws.Range(3, 11, 4, 13).Merge();
        th4.Value = "Glucose [mg/dL]";
        th4.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        th4.Style.Border.OutsideBorderColor = XLColor.FromArgb(0, 0, 0);

        var th5 = ws.Range(3, 14, 4, 16).Merge();
        th5.Value = "Weight [kg]";
        th5.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        th5.Style.Border.OutsideBorderColor = XLColor.FromArgb(0, 0, 0);

        var th6 = ws.Range(3, 17, 4, 19).Merge();
        th6.Value = "BMI [kg/m²]";
        th6.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        th6.Style.Border.OutsideBorderColor = XLColor.FromArgb(0, 0, 0);


        for (var i = 0; i < 18; i++)
        {
            var c = ws.Cell(5, i + 2);
            c.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            c.Style.Border.OutsideBorderColor = XLColor.FromArgb(0, 0, 0);
            c.Value = (i % 3) switch
            {
                0 => "Min",
                1 => "Max",
                2 => "Avg",
                _ => ""
            };
        }

        var row = 6;
        foreach (var r in report.Records)
        {
            var w = ws.Cell(row, 1);
            w.Value = $"{r.Year:0000}-{r.Month:00}";

            ws.Cell(row, 2).Value = r.Summary.SystolicSummary.Min;
            ws.Cell(row, 3).Value = r.Summary.SystolicSummary.Max;
            if (r.Summary.SystolicSummary.Avg is not null)
            {
                ws.Cell(row, 4).Value = Math.Round((decimal)r.Summary.SystolicSummary.Avg, 2);
            }
            ws.Cell(row, 4).Style.NumberFormat.Format = "0.00";

            ws.Cell(row, 5).Value = r.Summary.DiastolicSummary.Min;
            ws.Cell(row, 6).Value = r.Summary.DiastolicSummary.Max;
            if (r.Summary.DiastolicSummary.Avg is not null)
            {
                ws.Cell(row, 7).Value = Math.Round((decimal)r.Summary.DiastolicSummary.Avg, 2);
            }
            ws.Cell(row, 7).Style.NumberFormat.Format = "0.00";

            ws.Cell(row, 8).Value = r.Summary.PulseSummary.Min;
            ws.Cell(row, 9).Value = r.Summary.PulseSummary.Max;
            if (r.Summary.PulseSummary.Avg is not null)
            {
                ws.Cell(row, 10).Value = Math.Round((decimal)r.Summary.PulseSummary.Avg, 2);
            }
            ws.Cell(row, 10).Style.NumberFormat.Format = "0.00";

            ws.Cell(row, 11).Value = r.Summary.GlucoseSummary.Min;
            ws.Cell(row, 12).Value = r.Summary.GlucoseSummary.Max;
            if (r.Summary.GlucoseSummary.Avg is not null)
            {
                ws.Cell(row, 13).Value = Math.Round((decimal)r.Summary.GlucoseSummary.Avg, 2);
            }
            ws.Cell(row, 13).Style.NumberFormat.Format = "0.00";

            ws.Cell(row, 14).Value = r.Summary.WeightSummary.Min;
            ws.Cell(row, 15).Value = r.Summary.WeightSummary.Max;
            if (r.Summary.WeightSummary.Avg is not null)
            {
                ws.Cell(row, 16).Value = Math.Round((decimal)r.Summary.WeightSummary.Avg, 2);
            }
            ws.Cell(row, 16).Style.NumberFormat.Format = "0.00";

            ws.Cell(row, 17).Value = r.Summary.BmiSummary.Min;
            ws.Cell(row, 18).Value = r.Summary.BmiSummary.Max;
            if (r.Summary.BmiSummary.Avg is not null)
            {
                ws.Cell(row, 19).Value = Math.Round((decimal)r.Summary.BmiSummary.Avg, 2);
            }
            ws.Cell(row, 19).Style.NumberFormat.Format = "0.00";
            
            var c = ws.Range(row, 1,row,19);
            c.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            c.Style.Border.BottomBorderColor = XLColor.FromArgb(0, 0, 0);
            
            row++;
        }

        ws.Range(1,1,row,19).Style.Font.FontSize = 12;
        
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}