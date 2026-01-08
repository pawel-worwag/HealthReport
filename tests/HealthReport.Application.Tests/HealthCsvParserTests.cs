using System.Text;
using HealthReport.Application.FileParsers.BloodPresure.iHealth;

namespace HealthReport.Application.Tests;

public class HealthCsvParserTests
{
    private static Stream ToStream(string s) => new MemoryStream(Encoding.UTF8.GetBytes(s));

    [Fact]
    public void ValidSingleRecordWithHeader()
    {
        const string csv = @"Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note
""Jan 4, 2026"",09:02,124,79,67,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);

        Assert.Empty(result.Errors);
        Assert.Single(result.Data);
        var m = result.Data.ElementAt(0);
        Assert.Equal(new DateOnly(2026, 1, 4), m.MeasuredDate);
        Assert.Equal(new TimeOnly(9, 2), m.MeasuredTime);
        Assert.Equal(124, m.Systolic);
        Assert.Equal(79, m.Diastolic);
        Assert.Equal(67, m.Pulse);
        Assert.Null(m.Note);
    }

    [Fact]
    public void ValidSingleRecordWithoutHeader()
    {
        const string csv = @"""Jan 4, 2026"",09:02,124,79,67,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: false);

        Assert.Empty(result.Errors);
        Assert.Single(result.Data);
        var m = result.Data.ElementAt(0);
        Assert.Equal(new DateOnly(2026, 1, 4), m.MeasuredDate);
        Assert.Equal(new TimeOnly(9, 2), m.MeasuredTime);
        Assert.Equal(124, m.Systolic);
        Assert.Equal(79, m.Diastolic);
        Assert.Equal(67, m.Pulse);
        Assert.Null(m.Note);
    }

    [Fact]
    public void InvalidDate()
    {
        const string csv = @"Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note
""Jan 42, 2026"",09:02,124,79,67,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void InvalidTime()
    {
        const string csv = @"Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note
""Jan 4, 2026"",09:72,124,79,67,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
    }


    [Fact]
    public void MalformedNumber()
    {
        const string csv = @"Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note
""Jan 4, 2026"",09:02,12-4,79,67,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void InvalidColumnCount()
    {
        const string csv = @"Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note
""Jan 4, 2026"",,09:02,124,79,67,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
    }
    
    [Fact]
    public void EmptyStream()
    {
        var csv = string.Empty;
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: false);

        Assert.Empty(result.Errors);
        Assert.Empty(result.Data);
    }

    [Fact]
    public void FailFastPolicy()
    {
        const string csv = @"Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note
""Jan 4, 2026"",09:02,124,79,67,
""Jan 4, 2026"",09:01,126,78,67,
""Jan 3, 2026"",22:38,136,79,79,
""Jan 3, 2026"",22:36,13?5,76,76,
""Jan 3, 2026"",10:55,131,78,67,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Equal(3,result.Data.Count());
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void ErrorLineNumber()
    {
        const string csv = @"Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note
""Jan 4, 2026"",09:02,124,79,67,
""Jan 4, 2026"",09:01,126,78,67,
""Jan 3, 2026"",22:38,136,79,79,
""Jan 3, 2026"",22:36,13?5,76,76,
""Jan 3, 2026"",10:55,131,78,67,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Equal(5,result.Errors.First().Line);
    }
}