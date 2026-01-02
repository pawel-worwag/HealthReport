using System.Globalization;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.FileParsers.IHealth
{
    /// <summary>
    /// CSV importer for iHealth-exported files with headers: Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note
    /// Date format in sample: "Dec 28, 2025" (en-US)
    /// </summary>
    public static class BloodPressureCsvImporter
    {
        /// <summary>
        /// Parses a CSV file containing blood pressure measurements.
        /// Supports files with or without a header, controlled by the <c>hasHeader</c> parameter.
        /// </summary>
        /// <remarks>
        /// Returns a <see cref="ParseResult{T}"/>, where the <c>Data</c> property contains successfully
        /// parsed records and the <c>Errors</c> property contains parsing errors (line number and message).
        /// Malformed rows are skipped but recorded in <c>Errors</c>.
        /// </remarks>
        /// <param name="csvStream">Stream containing the CSV content.</param>
        /// <param name="hasHeader">Whether the first line is a header (default <c>true</c>).</param>
        /// <returns>A <see cref="ParseResult{BloodPressureMeasurement}"/> containing parsed measurements and any parse errors.</returns>
        public static ParseResult<BloodPressureMeasurement> ParseCsv(Stream csvStream, bool hasHeader = true)
        {
            
            var measurements = new List<BloodPressureMeasurement>();
            var errors = new List<ParseError>();
            
            using var reader = new StreamReader(csvStream);
            string? header = null;
            int lineNo = 0;
            
            if (hasHeader)
            {
                header = reader.ReadLine();
                lineNo++;
            }
            
            while (!reader.EndOfStream)
            {
                lineNo++;
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                // naive CSV split by comma - this file quotes dates but values do not contain commas otherwise
                var parts = SplitCsvLine(line);
                // Expected columns: Date, Time, SYS(mmHg), DIA(mmHg), Pulse(Beats/Min), Note
                if (parts.Length != 6)
                {
                    errors.Add(new ParseError { Line = lineNo, Message = $"Invalid number of columns: {parts.Length}" });
                    break;
                }

                try
                {
                    var dateStr = parts[0].Trim(' ', '"');
                    var timeStr = parts[1].Trim(' ', '"');

                    // parse date using en-US culture (e.g. "Dec 28, 2025")
                    var date = DateOnly.Parse(dateStr, CultureInfo.CreateSpecificCulture("en-US"));
                    var time = TimeOnly.Parse(timeStr, CultureInfo.InvariantCulture);

                    var systolic = int.Parse(parts[2]);
                    var diastolic = int.Parse(parts[3]);

                    int? pulse = null;
                    if (int.TryParse(parts[4], out var p))
                    {
                        pulse = p;
                    }
                    else
                    {
                        errors.Add(new ParseError { Line = lineNo, Message = $"Int parse error: {parts[4]}" });
                        break;
                    }

                    var note = parts[5].Trim();

                    var m = new BloodPressureMeasurement
                    {
                        MeasuredDate = date,
                        MeasuredTime = time,
                        Systolic = systolic,
                        Diastolic = diastolic,
                        Pulse = pulse,
                        Note = string.IsNullOrWhiteSpace(note) ? null : note
                    };
                    measurements.Add(m);
                }
                catch (Exception ex)
                {
                    errors.Add(new ParseError()
                    {
                        Line = lineNo,
                        Message = ex.Message
                    });
                }
            }

            return new ParseResult<BloodPressureMeasurement>()
            {
                Data = measurements, 
                Errors = errors
            };
        }

        // Very small CSV splitter that handles simple quoted fields
        private static string[] SplitCsvLine(string line)
        {
            var parts = new List<string>();
            bool inQuotes = false;
            var cur = string.Empty;
            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }
                if (c == ',' && !inQuotes)
                {
                    parts.Add(cur);
                    cur = string.Empty;
                    continue;
                }
                cur += c;
            }
            parts.Add(cur);
            return parts.ToArray();
        }
    }
}
