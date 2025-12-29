using System.Globalization;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.FileParsers.Contour
{
    /// <summary>
    /// CSV importer for Contour glucose meter exports.
    /// Expected header (Polish): #,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,...
    /// </summary>
    public static class ContourCsvImporter
    {
        /// <summary>
        /// Parses a Contour CSV stream into <see cref="BloodGlucoseMeasurement"/> objects.
        /// </summary>
        /// <param name="csvStream">CSV stream.</param>
        /// <param name="hasHeader">Whether the file contains a header row (default true).</param>
        public static ParseResult<BloodGlucoseMeasurement> ParseCsv(Stream csvStream, bool hasHeader = true)
        {
            var measurements = new List<BloodGlucoseMeasurement>();
            var errors = new List<ParseError>();

            using var reader = new StreamReader(csvStream);
            int lineNo = 0;
            if (hasHeader)
            {
                var h = reader.ReadLine();
                if (h == null) return new ParseResult<BloodGlucoseMeasurement> { Data = measurements, Errors = errors };
                lineNo++;
            }

            while (!reader.EndOfStream)
            {
                lineNo++;
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = SplitCsvLine(line);
                // Expected at least: index, datetime, value, meal marker, source, notes
                if (parts.Length < 3) continue;

                try
                {
                    var dateTimeStr = parts.Length > 1 ? parts[1].Trim(' ', '"') : string.Empty;

                    // parse datetime in format dd.MM.yyyy HH:mm:ss (e.g. "20.09.2025 06:54:49")
                    if (!DateTime.TryParseExact(dateTimeStr, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                    {
                        // Try pl-PL culture fallback
                        dt = DateTime.Parse(dateTimeStr, CultureInfo.GetCultureInfo("pl-PL"));
                    }

                    var date = DateOnly.FromDateTime(dt);
                    var time = TimeOnly.FromDateTime(dt);

                    var valueStr = parts.Length > 2 ? parts[2].Trim(' ', '"') : string.Empty;
                    if (!int.TryParse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var bg))
                        throw new FormatException($"Invalid BG value: '{valueStr}'");

                    var mealStr = parts.Length > 3 ? parts[3].Trim(' ', '"') : string.Empty;
                    var meal = MapMealMarker(mealStr);

                    var note = parts.Length > 5 ? parts[5].Trim() : null;

                    var m = new BloodGlucoseMeasurement
                    {
                        MeasuredDate = date,
                        MeasuredTime = time,
                        BGValue = bg,
                        Meal = meal,
                        Note = string.IsNullOrWhiteSpace(note) ? null : note
                    };

                    measurements.Add(m);
                }
                catch (Exception ex)
                {
                    errors.Add(new ParseError { Line = lineNo, Message = ex.Message });
                }
            }

            return new ParseResult<BloodGlucoseMeasurement> { Data = measurements, Errors = errors };
        }

        // Backwards-compatibility wrapper for the old class name
        public static class BloodGlucoseCsvImporter
        {
            public static ParseResult<BloodGlucoseMeasurement> ParseCsv(Stream csvStream, bool hasHeader = true)
                => ContourCsvImporter.ParseCsv(csvStream, hasHeader);
        }

        private static MealMarker MapMealMarker(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return MealMarker.Unknown;
            var t = s.Trim().ToLowerInvariant();
            return t switch
            {
                "na czczo" => MealMarker.Fasting,
                "przed posiłkiem" => MealMarker.BeforeMeal,
                "po posiłku" => MealMarker.AfterMeal,
                "bez znacznika" => MealMarker.Unknown,
                _ => MealMarker.Unknown
            };
        }

        // simple CSV splitter handling quoted fields
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
