using System.Text;
using HealthReport.Application.FileParsers.IHealth;

namespace HealthReport.Application.Tests;

public class HealthCsvImporterTests
{
    private static Stream ToStream(string s) => new MemoryStream(Encoding.UTF8.GetBytes(s));
    
    [Fact]
    public void ParseCsv_ValidSingleRow_ParsesMeasurement()
    {
        //   Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note
        //   "Dec 31, 2025",07:04,111,81,68,
        
        const string csv = "Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note\r\n" +
                           "\"Dec 31, 2025\",07:04,111,81,68,\r\n";

        var result = BloodPressureCsvImporter.ParseCsv(ToStream(csv), hasHeader: true);

        Assert.Empty(result.Errors);
        Assert.Single(result.Data);
        var m = result.Data.ElementAt(0);
        Assert.Equal(new DateOnly(2025, 12, 31), m.MeasuredDate);
        Assert.Equal(new TimeOnly(7, 4), m.MeasuredTime);
        Assert.Equal(111, m.Systolic);
        Assert.Equal(81, m.Diastolic);
        Assert.Equal(68, m.Pulse);
    }
    
    [Fact]
    public void ParseCsv_MultipleRows_ParsesAll()
    {
        ///   Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note
        ///   "Dec 31, 2025",07:04,111,81,68,note 123
        ///   "Dec 31, 2025",07:03,117,85,77,
        
        const string csv = "Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note\r\n" +
                           "\"Dec 31, 2025\",07:04,111,81,68,note 123\r\n" +
                           "\"Dec 31, 2025\",07:03,117,85,77,";

        var result = BloodPressureCsvImporter.ParseCsv(ToStream(csv), hasHeader: true);

        Assert.Empty(result.Errors);
        Assert.Equal(2, result.Data.Count());
    }
    
    [Fact]
    public void ParseCsv_MalformedNumber_RecordsErrorRow()
    {
        const string csv = "Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note\r\n" +
                           "\"Dec 31, 2025\",07:04,not-a-number,81,68,\r\n" +
                           "\"Dec 30, 2025\",06:53,119,81,57,\r\n";

        var result = BloodPressureCsvImporter.ParseCsv(ToStream(csv), hasHeader: true);

        Assert.Single(result.Errors);
        Assert.Empty(result.Data);
    }
    
    [Fact]
    public void ParseCsv_NoHeader_ParsesWhenHasHeaderFalse()
    {
        var csv = "\"Dec 31, 2025\",07:04,111,81,68,\r\n";
        var result = BloodPressureCsvImporter.ParseCsv(ToStream(csv), hasHeader: false);

        Assert.Empty(result.Errors);
        Assert.Single(result.Data);
        var m = result.Data.ElementAt(0);
        Assert.Equal(new DateOnly(2025, 12, 31), m.MeasuredDate);
    }
    
    [Fact]
    public void ParseCsv_EmptyStream_ReturnsEmptyResult()
    {
        var csv = string.Empty;
        var result = BloodPressureCsvImporter.ParseCsv(ToStream(csv), hasHeader: false);

        Assert.Empty(result.Errors);
        Assert.Empty(result.Data);
    }
}