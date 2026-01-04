using Xunit;
using HealthReport.Application.FileParsers;

namespace HealthReport.Application.Tests;

public class ParserHelperTests
{
    [Fact]
    public void SplitLine_SimpleCommaSeparated_ReturnsFields()
    {
        //   a,b,c
        var line = "a,b,c";
        var parts = CsvHelper.SplitCsvLine(line);
        Assert.Equal(new[] { "a", "b", "c" }, parts);
    }
    
    [Fact]
    public void SplitLine_QuotedFieldContainingSeparator_PreservesSeparatorInsideQuotes()
    {
        //   a,"b,c",d
        var line = "a,\"b,c\",d";
        var parts = CsvHelper.SplitCsvLine(line);
        Assert.Equal(new[] { "a", "b,c", "d" }, parts);
    }
    
    [Fact]
    public void SplitLine_EmptyFields_AreReturnedAsEmptyStrings()
    {
        //   a,,c,
        var line = "a,,c,";
        var parts = CsvHelper.SplitCsvLine(line);
        Assert.Equal(new[] { "a", "", "c", "" }, parts);
    }
    [Fact]
    
    public void SplitLine_EscapedQuotes_ProducesSingleQuote()
    {
        //   "d""e",f
        var line = "\"d\"\"e\",f";
        var parts = CsvHelper.SplitCsvLine(line);
        Assert.Equal(new[] { "d\"e", "f" }, parts);
    }
    
    [Fact]
    public void SplitLine_UnclosedQuote_ThrowsFormatException()
    {
        //   a,"unterminated,b
        var line = "a,\"unterminated,b";
        Assert.Throws<FormatException>(() => CsvHelper.SplitCsvLine(line));
    }
    
    [Fact]
    public void SplitLine_NullAndEmptyInputs_BehaveAsExpected()
    {
        Assert.Empty( CsvHelper.SplitCsvLine(string.Empty));
    }
}