using System.Text;
using HealthReport.Application.FileParsers.BloodGlucose.Contour;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Tests;

public class ContourCsvParserTests
{
    private static Stream ToStream(string s) => new MemoryStream(Encoding.UTF8.GetBytes(s));
    
    [Fact]
    public void ValidSingleRecordWithHeader()
    {
        const string csv = @"#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
1,""27.09.2025 08:36:03"",""108"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Errors);
        Assert.Single(result.Data);
        var m = result.Data.ElementAt(0);
        Assert.Equal(new DateOnly(2025, 9, 27), m.MeasuredDate);
        Assert.Equal(new TimeOnly(8, 36, 3), m.MeasuredTime);
        Assert.Equal(108, m.BGValue);
        Assert.Equal(MealMarker.BeforeMeal, m.Meal);
        Assert.Null(m.Note);
    }

    [Fact]
    public void ValidSingleRecordWithoutHeader()
    {
        const string csv = @"1,""27.09.2025 08:36:03"",""108"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: false);
        
        Assert.Empty(result.Errors);
        Assert.Single(result.Data);
        var m = result.Data.ElementAt(0);
        Assert.Equal(new DateOnly(2025, 9, 27), m.MeasuredDate);
        Assert.Equal(new TimeOnly(8, 36, 3), m.MeasuredTime);
        Assert.Equal(108, m.BGValue);
        Assert.Equal(MealMarker.BeforeMeal, m.Meal);
        Assert.Null(m.Note);
    }
    
    [Fact]
    public void ValidMultipleRecordWithHeader()
    {
        const string csv = @"#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
1,""27.09.2025 08:36:03"",""108"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""
2,""28.09.2025 08:01:00"",""119"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""
3,""28.09.2025 19:49:44"",""137"",""Po posiłku"",""Glukometr"","""","""","""","""",""Sadowa""";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Errors);
        Assert.Equal(3,result.Data.Count());
        var m = result.Data.ElementAt(2);
        Assert.Equal(new DateOnly(2025, 9, 28), m.MeasuredDate);
        Assert.Equal(new TimeOnly(19, 49, 44), m.MeasuredTime);
        Assert.Equal(137, m.BGValue);
        Assert.Equal(MealMarker.AfterMeal, m.Meal);
        Assert.Null(m.Note);
    }

    [Fact]
    public void InvalidDate()
    {
        const string csv = @"#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
1,""42.09.2025 08:36:03"",""108"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void InvalidTime()
    {
        const string csv = @"#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
1,""4.09.2025 08:71:03"",""108"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void MalformedNumber()
    {
        const string csv = @"#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
1,""4.09.2025 08:36:03"",""10-8"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void InvalidColumnCount()
    {
        const string csv = @"#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
1,""27.09.2025 08:36:03"",""108"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa"","""",""""";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void ValidMealMarker()
    {
        const string csv = @"#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
1,""27.09.2025 08:36:03"",""108"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""
2,""28.09.2025 19:49:44"",""137"",""Po posiłku"",""Glukometr"","""","""","""","""",""Sadowa""
3,""17.10.2025 04:54:30"",""143"",""Bez znacznika"",""Glukometr"","""","""","""","""",""""
4,""05.12.2025 07:08:22"",""130"",""Na czczo"",""Glukometr"","""","""","""","""",""Sadowa""";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Equal(MealMarker.BeforeMeal,result.Data.ElementAt(0).Meal);
        Assert.Equal(MealMarker.AfterMeal,result.Data.ElementAt(1).Meal);
        Assert.Equal(MealMarker.Unknown,result.Data.ElementAt(2).Meal);
        Assert.Equal(MealMarker.Fasting,result.Data.ElementAt(3).Meal);
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
        const string csv = @"#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
1,""27.09.2025 08:36:03"",""108"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""
2,""28.09.2025 08:01:00"",""119"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""
3,""28.09.2025 19:49:44"",""137"",""Po posiłku"",""Glukometr"","""","""","""","""",""Sadowa""
4,""29.09.2025 06:43:35"",""xyz"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""
5,""30.09.2025 06:53:51"",""131"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        Assert.Equal(3,result.Data.Count());
        Assert.NotEmpty(result.Errors);
    }
    
    [Fact]
    public void ErrorLineNumber()
    {
        const string csv = @"#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
1,""27.09.2025 08:36:03"",""108"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""
2,""28.09.2025 08:01:00"",""119"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""
3,""28.09.2025 19:49:44"",""137"",""Po posiłku"",""Glukometr"","""","""","""","""",""Sadowa""
4,""29.09.2025 06:43:35"",""xyz"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""
5,""30.09.2025 06:53:51"",""131"",""Przed posiłkiem"",""Glukometr"","""","""","""","""",""Sadowa""";
        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: true);
        
        Assert.Equal(5,result.Errors.First().Line);
    }
}