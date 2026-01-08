using Xunit;
using HealthReport.Application.FileParsers;

namespace HealthReport.Application.Tests;

public class ParserHelperTests
{
    [Fact]
    public void ValidSimpleCommaSeparated()
    {
        //   a,b,c
        var line = "a,b,c";
        var parts = CsvHelper.SplitCsvLine(line);
        Assert.Equal(new[] { "a", "b", "c" }, parts);
    }
    
    [Fact]
    public void ValiQuotedFieldContainingSeparator()
    {
        //   a,"b,c",d
        var line = "a,\"b,c\",d";
        var parts = CsvHelper.SplitCsvLine(line);
        Assert.Equal(new[] { "a", "b,c", "d" }, parts);
    }
    
    [Fact]
    public void ValidEmptyField()
    {
        //   a,,c,
        var line = "a,,c,";
        var parts = CsvHelper.SplitCsvLine(line);
        Assert.Equal(new[] { "a", "", "c", "" }, parts);
    }
    [Fact]
    
    public void ValidEscapedQuotes()
    {
        //   "d""e",f
        var line = "\"d\"\"e\",f";
        var parts = CsvHelper.SplitCsvLine(line);
        Assert.Equal(new[] { "d\"e", "f" }, parts);
    }
    
    [Fact]
    public void InvalidUnclosedQuote()
    {
        //   a,"unterminated,b
        var line = "a,\"unterminated,b";
        Assert.Throws<FormatException>(() => CsvHelper.SplitCsvLine(line));
    }
    
    [Fact]
    public void NullAndEmptyInputs()
    {
        Assert.Empty( CsvHelper.SplitCsvLine(string.Empty));
    }
}