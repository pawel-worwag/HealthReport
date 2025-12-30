using ClosedXML.Excel;

namespace HealthReport.Application.Handlers.Reports.Simple;

public static class SimpleReportToXlsx
{
    public static byte[] Export(SimpleReportDto report)
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
        
        ws.Cell(1,1).Value = $"Date:";
        ws.Cell(1,1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        ws.Cell(1,2).Value = $"{report.Year:0000}-{report.Month:00}";
        var title = ws.Range(1,1,1,12);
        title.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        title.Style.Border.BottomBorderColor = XLColor.FromArgb(0, 0, 0);
        
        //summary
        ws.Cell(3, 1).Value = "SUMMARY";
        ws.Cell(3,1).Style.Font.Bold = true;
        
        ws.Cell(4, 2).Value = "Min";
        ws.Cell(4, 3).Value = "Max";
        ws.Cell(4, 4).Value = "Avg";
        
        var h1 =ws.Range(4,1,4,4);
        h1.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        h1.Style.Border.BottomBorderColor = XLColor.FromArgb(0, 0, 0);
        
        ws.Cell(5, 1).Value = "Blood pressure - Systolic [mmHg]";
        ws.Cell(5,2).Value=report.Summary.SystolicSummary.Min;
        ws.Cell(5,3).Value=report.Summary.SystolicSummary.Max;
        if (report.Summary.SystolicSummary.Avg is not null)
        {
            ws.Cell(5, 4).Value = Math.Round((decimal)report.Summary.SystolicSummary.Avg,0);
        }

        ws.Cell(6, 1).Value = "Blood pressure - Diastolic [mmHg]";
        ws.Cell(6,2).Value=report.Summary.DiastolicSummary.Min;
        ws.Cell(6,3).Value=report.Summary.DiastolicSummary.Max;
        if (report.Summary.DiastolicSummary.Avg is not null)
        {
            ws.Cell(6, 4).Value = Math.Round((decimal)report.Summary.DiastolicSummary.Avg,0);
        }
        
        ws.Cell(7, 1).Value = "Pulse [bpm]";
        ws.Cell(7,2).Value=report.Summary.PulseSummary.Min;
        ws.Cell(7,3).Value=report.Summary.PulseSummary.Max;
        if (report.Summary.PulseSummary.Avg is not null)
        {
            ws.Cell(7, 4).Value = Math.Round((decimal)report.Summary.PulseSummary.Avg,0);
        }
        
        ws.Cell(8, 1).Value = "Glucose [mg/dL]";
        ws.Cell(8,2).Value=report.Summary.GlucoseSummary.Min;
        ws.Cell(8,3).Value=report.Summary.GlucoseSummary.Max;
        if (report.Summary.GlucoseSummary.Avg is not null)
        {
            ws.Cell(8, 4).Value = Math.Round((decimal)report.Summary.GlucoseSummary.Avg,0);
        }
        
        ws.Cell(9, 1).Value = "Weight [kg]";
        ws.Cell(9,2).Value=report.Summary.WeightSummary.Min;
        ws.Cell(9,3).Value=report.Summary.WeightSummary.Max;
        if (report.Summary.WeightSummary.Avg is not null)
        {
            ws.Cell(9, 4).Value = Math.Round((decimal)report.Summary.WeightSummary.Avg,0);
        }
        
        ws.Cell(10, 1).Value = "BMI [kg/m²]";
        ws.Cell(10,2).Value=report.Summary.BmiSummary.Min;
        ws.Cell(10,3).Value=report.Summary.BmiSummary.Max;
        if (report.Summary.BmiSummary.Avg is not null)
        {
            ws.Cell(10, 4).Value = Math.Round((decimal)report.Summary.BmiSummary.Avg,0);
        }

        var h12 =ws.Range(5,1,10,4);
        h12.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        h12.Style.Border.BottomBorderColor = XLColor.FromArgb(0, 0, 0);
        
        //details
        ws.Cell(12,1).Value = "DETAILS";
        ws.Cell(12,1).Style.Font.Bold = true;
        
        ws.Cell(13,1).Value = "Date";
        ws.Cell(13,1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        var h21 =ws.Range(13, 1, 14, 1);
        h21.Merge();
        h21.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        h21.Style.Border.BottomBorderColor = XLColor.FromArgb(0, 0, 0);
        
        ws.Cell(13,2).Value = "Blood pressure";
        ws.Range(13, 2, 13, 5).Merge();
        ws.Cell(14,2).Value = "Time";
        ws.Cell(14,3).Value = "Sys [mmHg]";
        ws.Cell(14,4).Value = "Dia [mmHg]";
        ws.Cell(14,5).Value = "Pulse [mmHg]";
        var h22 = ws.Range(13, 2, 14, 5);
        h22.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        h22.Style.Border.BottomBorderColor = XLColor.FromArgb(0, 0, 0);
        
        ws.Cell(13,6).Value = "Glucose";
        ws.Range(13, 6, 13, 9).Merge();
        ws.Cell(14,6).Value = "Time";
        ws.Cell(14,7).Value = "Glucose [mg/dL]";
        ws.Cell(14,8).Value = "Meal";
        ws.Cell(14,9).Value = "Note";
        var h23 = ws.Range(13, 6, 14, 9);
        h23.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        h23.Style.Border.BottomBorderColor = XLColor.FromArgb(0, 0, 0);
        
        ws.Cell(13,10).Value = "Weight";
        ws.Range(13, 10, 13, 12).Merge();
        ws.Cell(14,10).Value = "Time";
        ws.Cell(14,11).Value = "Weight [kg]";
        ws.Cell(14,12).Value = "BMI [kg/m²]";
        var h24 = ws.Range(13, 10, 14, 12);
        h24.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        h24.Style.Border.BottomBorderColor = XLColor.FromArgb(0, 0, 0);

        var row = 15;
        foreach (var d in report.Details)
        {
            var bCount = d.BloodPressure.Count();
            var gCount = d.Glucose.Count();
            var wCount = d.Weight.Count();
            var maxCount = Math.Max(bCount, Math.Max(gCount, wCount));
            if (maxCount == 0)
            {
                ws.Cell(row, 1).Value = d.Date.ToString("yyyy-MM-dd");
                var h =ws.Range(row,1,row,12);
                h.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                h.Style.Border.BottomBorderColor = XLColor.FromArgb(0, 0, 0);
                row++;
            }
            else
            {
                var rowEnd = row + maxCount - 1;
                var h =ws.Range(row,1,rowEnd,12);
                h.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                h.Style.Border.BottomBorderColor = XLColor.FromArgb(0, 0, 0);
                for (int i = 0; i < maxCount; i++)
                {
                    if (i == 0)
                    {
                        ws.Cell(row, 1).Value = d.Date.ToString("yyyy-MM-dd");
                        var r =ws.Range(row,1,row+maxCount-1,1);
                        r.Merge();
                        r.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    }

                    if (i < bCount)
                    {
                        ws.Cell(row, 2).Value = d.BloodPressure[i].Time.ToString("HH:mm");
                        ws.Cell(row, 3).Value = d.BloodPressure[i].Systolic;
                        ws.Cell(row, 4).Value = d.BloodPressure[i].Diastolic;
                        ws.Cell(row, 5).Value = d.BloodPressure[i].Pulse;
                    }

                    if (i < gCount)
                    {
                        ws.Cell(row, 6).Value = d.Glucose[i].Time.ToString("HH:mm");
                        ws.Cell(row, 7).Value = d.Glucose[i].Value;
                        ws.Cell(row, 8).Value = d.Glucose[i].Meal;
                        ws.Cell(row, 9).Value = d.Glucose[i].Note;
                    }

                    if (i < wCount)
                    {
                        ws.Cell(row, 10).Value = d.Weight[i].Time.ToString("HH:mm");
                        ws.Cell(row, 11).Value = d.Weight[i].WeightKg;
                        ws.Cell(row, 12).Value = d.Weight[i].Bmi;
                    }
                    row++;
                }
            }
        }
        
        ws.Range(1,1,row,12).Style.Font.FontSize = 12;
        //ws.PageSetup.PrintAreas.Add(1,1,row,12);
        
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}