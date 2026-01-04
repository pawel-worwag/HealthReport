using System.Text;
using HealthReport.Application.FileParsers.Weight.Garmin;

namespace HealthReport.Application.Tests;

public class GarminCsvParserTests
{
    private static Stream ToStream(string s) => new MemoryStream(Encoding.UTF8.GetBytes(s));
    
    [Fact]
    public void ParseCsv_DateLineFollowedByTimeRow_AppliesDateToTimeRow()
    {
        var csv =
            "\" 2025 Gru 28\",\r\n" +
            "10:20 AM,98.8 kg,0.6 kg,33.4,33.2 %,36.9 kg,5.3 kg,48.7 %,\r\n";

        var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: false);

        Assert.Empty(result.Errors);
        Assert.Single(result.Data);

        var m = result.Data.ElementAt(0);
        Assert.Equal(new DateOnly(2025, 12, 28), m.MeasuredDate);
        Assert.Equal(new TimeOnly(10, 20), m.MeasuredTime);
        Assert.Equal(98.8m, m.WeightKg);
    }
    
    

        [Fact]
        public void ParseCsv_MultipleTimeRowsAfterDate_RemembersDateForAllRows()
        {
            var csv =
                "\" 2025 Gru 26\",\r\n" +
                "11:35 AM,98.2 kg,0.4 kg,33.2,33 %,36.8 kg,5.3 kg,48.9 %,\r\n" +
                "12:22 PM,97.1 kg,1.1 kg,32.8,33.3 %,36.5 kg,5.2 kg,48.7 %,\r\n";

            var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: false);

            Assert.Empty(result.Errors);
            Assert.Equal(2, result.Data.Count());
            Assert.All(result.Data, d => Assert.Equal(new DateOnly(2025, 12, 26), d.MeasuredDate));
            Assert.Contains(result.Data, d => d.MeasuredTime == new TimeOnly(11, 35));
            Assert.Contains(result.Data, d => d.MeasuredTime == new TimeOnly(12, 22));
        }

        [Fact]
        public void ParseCsv_Mixed12hAnd24hTimeFormats_ParsesBothFormats()
        {
            var csv =
                "\" 2025 Gru 24\",\r\n" +
                "9:35 AM,97.8 kg,0.6 kg,33.1,33 %,36.7 kg,5.3 kg,48.9 %,\r\n" +
                "16:10,96.5 kg,0.3 kg,32.6,33.2 %,36.3 kg,5.2 kg,48.8 %,\r\n";

            var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: false);

            Assert.Empty(result.Errors);
            Assert.Equal(2, result.Data.Count());
            Assert.Contains(result.Data, d => d.MeasuredTime == new TimeOnly(9, 35));
            Assert.Contains(result.Data, d => d.MeasuredTime == new TimeOnly(16, 10));
        }

        [Fact]
        public void ParseCsv_ParsesNumericFieldsStrippingUnits_AndPercentFields()
        {
            var csv =
                "\" 2025 Gru 23\",\r\n" +
                "7:18 AM,97.2 kg,0.3 kg,32.9,32.9 %,36.5 kg,5.3 kg,49.0 %,\r\n";

            var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: false);

            Assert.Empty(result.Errors);
            Assert.Single(result.Data);

            var m = result.Data.ElementAt(0);
            Assert.Equal(97.2m, m.WeightKg);
            Assert.Equal(32.9m, m.BodyFatPercentage);
            // optional: assert other numeric conversions if properties exist
        }

        [Fact]
        public void ParseCsv_MissingOrInvalidRows_AreReportedAsErrors_AndOtherRowsParsed()
        {
            var csv =
                "\" 2025 Gru 21\",\r\n" +
                "W\r\n" + // malformed line in real export (should produce error)
                "12:22 PM,97.1 kg,1.1 kg,32.8,33.3 %,36.5 kg,5.2 kg,48.7 %,\r\n";

            var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: false);

            Assert.NotEmpty(result.Errors);
            Assert.Single(result.Data);
            Assert.Equal(97.1m, result.Data.ElementAt(0).WeightKg);
        }

        [Fact]
        public void ParseCsv_HandlesPolishMonthAbbreviationsAcrossMonths()
        {
            var csv =
                "\" 2025 Lis 28\",\r\n" +
                "6:53 AM,96.9 kg,0.4 kg,32.8,33.1 %,36.4 kg,5.2 kg,48.8 %,\r\n" +
                "\" 2025 Paź 30\",\r\n" +
                "7:01 AM,96.1 kg,0.2 kg,32.5,33.4 %,36.2 kg,5.1 kg,48.6 %,\r\n";

            var result = CsvParser.ParseCsv(ToStream(csv), hasHeader: false);

            Assert.Empty(result.Errors);
            Assert.Equal(2, result.Data.Count());
            Assert.Contains(result.Data, d => d.MeasuredDate == new DateOnly(2025, 11, 28));
            Assert.Contains(result.Data, d => d.MeasuredDate == new DateOnly(2025, 10, 30));
        }
}