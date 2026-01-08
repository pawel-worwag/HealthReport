using System.Text;
using HealthReport.Application.FileParsers.Weight.Garmin;

namespace HealthReport.Application.Tests;

public class GarminCsvParserTests
{
    private static Stream ToStream(string s) => new MemoryStream(Encoding.UTF8.GetBytes(s));

    [Fact]
    public void ValidSingleRecordWithHeader()
    {
        const string csv =
            @"Czas,Ciężar,Zmiana,BMI,Tkanka tłuszczowa,Masa mięśni szkieletowych,Masa kostna,Woda w organizmie,
"" 2026 Sty 4"",
10:12 AM,97.8 kg,0.0 kg,33,33.7 %,36.7 kg,5.2 kg,48.4 %,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);

        Assert.Empty(result.Errors);
        Assert.Single(result.Data);
        var m = result.Data.ElementAt(0);
        Assert.Equal(new DateOnly(2026, 1, 4), m.MeasuredDate);
        Assert.Equal(new TimeOnly(10, 12), m.MeasuredTime);
        Assert.Equal(97.8m, m.WeightKg);
        Assert.Equal(0.0m, m.WeightChangeKg);
        Assert.Equal(33m, m.BMI);
    }

    [Fact]
        public void ValidSingleRecordWithoutHeader()
        {
            const string csv = @""" 2026 Sty 4"",
10:12 AM,97.8 kg,0.0 kg,33,33.7 %,36.7 kg,5.2 kg,48.4 %,";
            var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: false);
        
            Assert.Empty(result.Errors);
            Assert.Single(result.Data);
            var m = result.Data.ElementAt(0);
            Assert.Equal(new DateOnly(2026, 1, 4), m.MeasuredDate);
            Assert.Equal(new TimeOnly(10, 12), m.MeasuredTime);
            Assert.Equal(97.8m, m.WeightKg);
            Assert.Equal(0.0m, m.WeightChangeKg);
            Assert.Equal(33m, m.BMI);
    }

    [Fact]
    public void ValidMultipleRecordWithHeader()
    {
        const string csv =
            @"Czas,Ciężar,Zmiana,BMI,Tkanka tłuszczowa,Masa mięśni szkieletowych,Masa kostna,Woda w organizmie,
"" 2026 Sty 4"",
10:12 AM,97.8 kg,0.0 kg,33,33.7 %,36.7 kg,5.2 kg,48.4 %,
"" 2026 Sty 3"",
10:51 AM,97.8 kg,0.6 kg,33.1,33.1 %,36.7 kg,5.3 kg,48.8 %,
"" 2026 Sty 1"",
9:17 AM,98.3 kg,0.5 kg,33.2,33.3 %,36.8 kg,5.3 kg,48.7 %,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);

        Assert.Empty(result.Errors);
        Assert.Equal(3,result.Data.Count());
        var m = result.Data.ElementAt(2);
        Assert.Equal(new DateOnly(2026, 1, 1), m.MeasuredDate);
        Assert.Equal(new TimeOnly(9, 17), m.MeasuredTime);
        Assert.Equal(98.3m, m.WeightKg);
        Assert.Equal(0.5m, m.WeightChangeKg);
        Assert.Equal(33.2m, m.BMI);
    }

    [Fact]
    public void ValidSingleRecordPartialData()
    {
        const string csv = @"Czas,Ciężar,Zmiana,BMI,Tkanka tłuszczowa,Masa mięśni szkieletowych,Masa kostna,Woda w organizmie,
"" 2026 Sty 4"",
5:43 PM,98.4 kg,0.1 kg,33.3,--,--,--,--,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Errors);
        Assert.Single(result.Data);
        var m = result.Data.ElementAt(0);
        Assert.Equal(new DateOnly(2026, 1, 4), m.MeasuredDate);
        Assert.Equal(new TimeOnly(17, 43), m.MeasuredTime);
        Assert.Equal(98.4m, m.WeightKg);
        Assert.Equal(0.1m, m.WeightChangeKg);
        Assert.Equal(33.3m, m.BMI);
        Assert.Null(m.BodyFatPercentage);
        Assert.Null(m.SkeletalMuscleMassKg);
        Assert.Null(m.BodyWaterPercentage);
    }

    [Fact]
    public void InvalidDate()
    {
        const string csv = @"Czas,Ciężar,Zmiana,BMI,Tkanka tłuszczowa,Masa mięśni szkieletowych,Masa kostna,Woda w organizmie,
"" 2026 Sty 42"",
10:12 AM,97.8 kg,0.0 kg,33,33.7 %,36.7 kg,5.2 kg,48.4 %,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void InvalidTime()
    {
        const string csv = @"Czas,Ciężar,Zmiana,BMI,Tkanka tłuszczowa,Masa mięśni szkieletowych,Masa kostna,Woda w organizmie,
"" 2026 Sty 4"",
10:72 AM,97.8 kg,0.0 kg,33,33.7 %,36.7 kg,5.2 kg,48.4 %,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void MalformedNumber()
    {
        const string csv = @"Czas,Ciężar,Zmiana,BMI,Tkanka tłuszczowa,Masa mięśni szkieletowych,Masa kostna,Woda w organizmie,
"" 2026 Sty 4"",
10:12 AM,97.8 kg,0.0 kg,3-3,33.7 %,36.7 kg,5.2 kg,48.4 %,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void InvalidColumnCount()
    {
        const string csv = @"Czas,Ciężar,Zmiana,BMI,Tkanka tłuszczowa,Masa mięśni szkieletowych,Masa kostna,Woda w organizmie,
"" 2026 Sty 4"",
10:12 AM,97.8 kg,0.0 kg,33,,33.7 %,36.7 kg,5.2 kg,48.4 %,";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
    }
    
    [Fact]
    public void MultipleTimeRowsAfterDate()
    {
        const string csv =@""" 2025 Gru 26"",
11:35 AM,98.2 kg,0.4 kg,33.2,33 %,36.8 kg,5.3 kg,48.9 %,
12:22 PM,97.1 kg,1.1 kg,32.8,33.3 %,36.5 kg,5.2 kg,48.7 %,";

        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: false);

        Assert.Empty(result.Errors);
        Assert.Equal(2, result.Data.Count());
        Assert.All(result.Data, d => Assert.Equal(new DateOnly(2025, 12, 26), d.MeasuredDate));
        Assert.Contains(result.Data, d => d.MeasuredTime == new TimeOnly(11, 35));
        Assert.Contains(result.Data, d => d.MeasuredTime == new TimeOnly(12, 22));
    }

    [Fact]
    public void DateLineFollowedByTimeRow()
    {
        const string csv =@"11:35 AM,98.2 kg,0.4 kg,33.2,33 %,36.8 kg,5.3 kg,48.9 %,
"" 2025 Gru 26"",
12:22 PM,97.1 kg,1.1 kg,32.8,33.3 %,36.5 kg,5.2 kg,48.7 %,";

        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: false);
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
}