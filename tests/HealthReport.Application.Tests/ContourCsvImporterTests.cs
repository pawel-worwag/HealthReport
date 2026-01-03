using System.Text;
using HealthReport.Application.FileParsers.Contour;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Tests;

public class ContourCsvImporterTests
{
    static Stream ToStream(string s) => new MemoryStream(Encoding.UTF8.GetBytes(s));
    
    [Fact]
    public void ParseCsv_ValidLine_ParsesMeasurement()
    {
        // #,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
        // 2,"24.09.2025 06:50:04","122","Na czczo","Glukometr","","","","","Sadowa"
        
        var csv = "#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja\r\n" +
                  "1,\"24.09.2025 06:50:04\",\"122\",\"Na czczo\",\"Glukometr\",\"\",\"\",\"\",\"\",\"Sadowa\"";

        var result = ContourCsvImporter.ParseCsv(ToStream(csv), hasHeader: true);

        Assert.Empty(result.Errors);
        Assert.Single(result.Data);
        var m = result.Data.ElementAt(0);
        Assert.Equal(new DateOnly(2025, 9, 24), m.MeasuredDate);
        Assert.Equal(new TimeOnly(6, 50, 4), m.MeasuredTime);
        Assert.Equal(122, m.BGValue);
        Assert.Equal(MealMarker.Fasting, m.Meal);
        Assert.Null(m.Note);
    }
    
    [Fact]
    public void ParseCsv_InvalidColumnCount_ReportsError()
    {
        var csv = "#,Data,BG\r\n" +
                  "1,01.02.2025 08:30:00,120\r\n";

        var result = ContourCsvImporter.ParseCsv(ToStream(csv), hasHeader: true);

        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
        Assert.Equal(2, result.Errors.ElementAt(0).Line);
    }
    
    [Fact]
    public void ParseCsv_InvalidDate_ReportsErrorAndStops()
    {
        //   #,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
        //   1,invalid-date,120,na czczo,device,,act,100,,loc
        //   2,01.02.2025 09:00:00,110,na czczo,device,,act,100,,loc
        
        var csv = "#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja\r\n" +
                  "1,invalid-date,120,na czczo,device,,act,100,,loc\r\n" +
                  "2,01.02.2025 09:00:00,110,na czczo,device,,act,100,,loc\r\n";

        var result = ContourCsvImporter.ParseCsv(ToStream(csv), hasHeader: true);

        Assert.Empty(result.Data); // importer breaks on parse error
        Assert.Single(result.Errors);
        Assert.Contains("Invalid datetime format", result.Errors.ElementAt(0).Message);
    }
    
    [Fact]
    public void ParseCsv_InvalidBgValue_ReportsErrorAndStops()
    {
        //   #,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
        //   1,01.02.2025 08:30:00,not-a-number,na czczo,device,,act,100,,loc
        //   2,01.02.2025 09:00:00,110,na czczo,device,,act,100,,loc
        
        var csv = "#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja\r\n" +
                  "1,01.02.2025 08:30:00,not-a-number,na czczo,device,,act,100,,loc\r\n" +
                  "2,01.02.2025 09:00:00,110,na czczo,device,,act,100,,loc";

        var result = ContourCsvImporter.ParseCsv(ToStream(csv), hasHeader: true);

        Assert.Empty(result.Data);
        Assert.Single(result.Errors);
        Assert.Contains("Invalid BGValue format", result.Errors.ElementAt(0).Message);
    }
    
    [Fact]
    public void ParseCsv_MealMarkerMapping_WorksForKnownValues()
    {
        //   #,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
        //   1,01.02.2025 08:30:00,100,przed posiłkiem,device,,act,100,,loc
        //   2,01.02.2025 09:30:00,110,po posiłku,device,,act,100,,loc
        //   3,01.02.2025 09:30:00,110,Na czczo,device,,act,100,,loc
        
        var csv = "#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja\r\n" +
                  "1,01.02.2025 08:30:00,100,Przed posiłkiem,device,,act,100,,loc\r\n" +
                  "2,01.02.2025 09:30:00,110,Po posiłku,device,,act,100,,loc\r\n" +
                  "3,01.02.2025 09:30:00,110,Na czczo,device,,act,100,,loc";

        var result = ContourCsvImporter.ParseCsv(ToStream(csv), hasHeader: true);

        // depending on current implementation behavior (it may stop on first error),
        // the test asserts intended behaviour: both lines parsed successfully.
        Assert.Empty(result.Errors);
        Assert.Equal(3, result.Data.Count());
        Assert.Equal(MealMarker.BeforeMeal, result.Data.ElementAt(0).Meal);
        Assert.Equal(MealMarker.AfterMeal, result.Data.ElementAt(1).Meal);
        Assert.Equal(MealMarker.Fasting, result.Data.ElementAt(2).Meal);
    }
    
    [Fact]
    public void ParseCsv_MultipleValidLines_ParsesAll()
    {
        var csv = "#,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja\r\n" +
                  "1,01.02.2025 08:30:00,120,na czczo,device,,act,100,,loc\r\n" +
                  "2,01.02.2025 09:00:00,115,na czczo,device,,act,100,,loc\r\n" +
                  "3,01.02.2025 09:30:00,110,Na czczo,device,,act,100,,loc";

        var result = ContourCsvImporter.ParseCsv(ToStream(csv), hasHeader: true);

        Assert.Empty(result.Errors);
        Assert.Equal(3, result.Data.Count());
    }
       
    [Fact]
    public void ParseCsv_MultipleValidLines_WithoutHeader_ParsesAll()
    {
        var csv = "1,01.02.2025 08:30:00,120,na czczo,device,,act,100,,loc\r\n" +
                  "2,01.02.2025 09:00:00,115,na czczo,device,,act,100,,loc\r\n" +
                  "3,01.02.2025 09:30:00,110,Na czczo,device,,act,100,,loc";

        var result = ContourCsvImporter.ParseCsv(ToStream(csv), hasHeader: false);

        Assert.Empty(result.Errors);
        Assert.Equal(3, result.Data.Count());
    }
}