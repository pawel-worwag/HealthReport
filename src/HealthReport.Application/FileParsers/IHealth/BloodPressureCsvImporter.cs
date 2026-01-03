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

                var parts = CsvHelper.SplitCsvLine(line);
                
                // Expected columns: Date,Time,SYS(mmHg),DIA(mmHg),Pulse(Beats/Min),Note
                if (parts.Length != 6)
                {
                    errors.Add(new ParseError { Line = lineNo, Message = $"Invalid number of columns: {parts.Length}" });
                    break;
                }

                try
                {
                    DateOnly? measuredDate = null;
                    TimeOnly? measuredTime = null;
                    int? systolic = null;
                    int? diastolic = null;
                    int? pulse = null;
                    string? note = null;
                    
                    // parse date using en-US culture (e.g. "Dec 28, 2025")
                    if (DateOnly.TryParse(parts[0].Trim(' '), CultureInfo.CreateSpecificCulture("en-US"), out var parsedDate))
                    {
                        measuredDate = parsedDate;
                    }
                    else
                    {
                        errors.Add(new ParseError { Line = lineNo, Message = $"Invalid date format: value='{parts[0]}'" });
                        break;
                    }
                    
                    if (TimeOnly.TryParse(parts[1].Trim(' '), CultureInfo.InvariantCulture, out var parsedTime))
                    {
                        measuredTime = parsedTime;
                    }
                    else
                    {
                        errors.Add(new ParseError { Line = lineNo, Message = $"Invalid time format: value='{parts[1]}'" });
                        break;
                    }

                    if (int.TryParse(parts[2], out var s))
                    {
                        systolic = s;
                    }
                    else
                    {
                        errors.Add(new ParseError { Line = lineNo, Message = $"Int parse error: value={parts[2]}" });
                        break;
                    }

                    if (int.TryParse(parts[3], out var d))
                    {
                        diastolic = d;
                    }
                    else
                    {
                        errors.Add(new ParseError { Line = lineNo, Message = $"Int parse error: value={parts[2]}" });
                        break;
                    }
                    
                    if (int.TryParse(parts[4], out var p))
                    {
                        pulse = p;
                    }
                    else
                    {
                        errors.Add(new ParseError { Line = lineNo, Message = $"Int parse error: value={parts[2]}" });
                        break;
                    }


                    note = parts[5].Trim();

                    var m = new BloodPressureMeasurement
                    {
                        MeasuredDate = (DateOnly)measuredDate,
                        MeasuredTime = (TimeOnly)measuredTime,
                        Systolic = (int)systolic,
                        Diastolic = (int)diastolic,
                        Pulse = pulse,
                        Note = string.IsNullOrWhiteSpace(note) ? null : note
                    };
                    measurements.Add(m);
                }
                catch (Exception ex)
                {
                    errors.Add(new ParseError { Line = lineNo, Message = ex.Message });
                }
            }

            return new ParseResult<BloodPressureMeasurement> { Data = measurements, Errors = errors };
        }
    }
}
