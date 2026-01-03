using System.Globalization;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.FileParsers.Garmin
{
    /// <summary>
    /// Simple CSV importer for Garmin Connect weight exports.
    /// It expects a header row and columns containing at least: Date, Time, Weight(kg).
    /// The importer is tolerant: missing optional columns (BMI, fat, muscle, water, change) are allowed.
    /// </summary>
    public static class WeightCsvImporter
    {
        public static ParseResult<WeightMeasurement> ParseCsv(Stream csvStream, bool hasHeader = true)
        {
            var list = new List<WeightMeasurement>();
            var errors = new List<ParseError>();

            using var reader = new StreamReader(csvStream);
            int lineNo = 0;

            string? headerLine = null;
            string[] headers = Array.Empty<string>();
            if (hasHeader)
            {
                headerLine = reader.ReadLine();
                lineNo++;
                if (headerLine != null)
                    headers = CsvHelper.SplitCsvLine(headerLine);
            }

            DateOnly? currentDate = null;

            while (!reader.EndOfStream)
            {
                lineNo++;
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = CsvHelper.SplitCsvLine(line);

                if (parts.Length != 2 && parts.Length != 9)
                {
                    errors.Add(new ParseError { Line = lineNo, Message = $"Invalid number of columns: {parts.Length}" });
                    break;
                }
                
                // detect lines that contain only a date in first column and empty others
                if (parts.Length >= 1)
                {
                    var maybeDate = parts[0].Trim(' ', '"');
                    var othersEmpty = true;
                    for (int ii = 1; ii < parts.Length; ii++) if (!string.IsNullOrWhiteSpace(parts[ii])) { othersEmpty = false; break; }
                    if (othersEmpty && TryParseDateOnly(maybeDate, out var parsedDate))
                    {
                        currentDate = parsedDate;
                        continue;
                    }
                }

                try
                {
                    // Heuristics: try to find Date and Time columns or a combined DateTime column
                    string? dateStr = null;
                    string? timeStr = null;
                    string? dateTimeStr = null;
                    string? weightStr = null;
                    string? changeStr = null;
                    string? bmiStr = null;
                    string? fatStr = null;
                    string? muscleStr = null;
                    string? waterStr = null;
                    string? sourceStr = null;
                    string? noteStr = null;

                    for (int i = 0; i < parts.Length; i++)
                    {
                        var h = headers.Length > i ? headers[i].Trim().ToLowerInvariant() : string.Empty;
                        var p = parts[i].Trim(' ', '"');
                        if (string.IsNullOrEmpty(h))
                        {
                            // fallback: try common column names by position later
                            continue;
                        }

                        if (h.Contains("date") && dateStr == null) dateStr = p;
                        else if ((h.Contains("time") || h.Contains("czas")) && timeStr == null) timeStr = p;
                        else if (h.Contains("datetime") || h.Contains("date time") || h.Contains("date/ time")) dateTimeStr = p;
                        else if (h.Contains("weight") && weightStr == null) weightStr = p;
                        else if (h.Contains("change") && changeStr == null) changeStr = p;
                        else if (h.Contains("bmi") && bmiStr == null) bmiStr = p;
                        else if ((h.Contains("fat") || h.Contains("bodyfat") || h.Contains("tłuszcz")) && fatStr == null) fatStr = p;
                        else if ((h.Contains("muscle") || h.Contains("skeletal") || h.Contains("mięśn")) && muscleStr == null) muscleStr = p;
                        else if ((h.Contains("water") || h.Contains("woda")) && waterStr == null) waterStr = p;
                        else if (h.Contains("source") && sourceStr == null) sourceStr = p;
                        else if (h.Contains("note") || h.Contains("uwagi")) noteStr = p;
                    }

                    // Fallback by position if weight not found (weight typically in column 2)
                    if (weightStr == null && parts.Length > 0)
                        weightStr = (parts.Length > 1 ? parts[1] : parts[0]).Trim(' ', '"');

                    DateOnly date;
                    TimeOnly time;

                    if (!string.IsNullOrEmpty(dateTimeStr))
                    {
                        if (!TryParseDateTime(dateTimeStr, out date, out time))
                            throw new FormatException($"Invalid datetime: '{dateTimeStr}'");
                    }
                    else if (!string.IsNullOrEmpty(dateStr))
                    {
                        if (!DateOnly.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        {
                            if (!DateOnly.TryParse(dateStr, new CultureInfo("pl-PL"), DateTimeStyles.None, out date))
                                throw new FormatException($"Invalid date: '{dateStr}'");
                        }

                        if (!string.IsNullOrEmpty(timeStr))
                        {
                            if (!TimeOnly.TryParse(timeStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
                            {
                                if (!TimeOnly.TryParse(timeStr, new CultureInfo("pl-PL"), DateTimeStyles.None, out time))
                                    time = TimeOnly.MinValue;
                            }
                        }
                        else
                        {
                            time = TimeOnly.MinValue;
                        }
                    }
                    else if (!string.IsNullOrEmpty(timeStr) && currentDate.HasValue)
                    {
                        // Use last seen date with this time
                        if (!TryParseTimeOnly(timeStr, out time))
                            throw new FormatException($"Invalid time: '{timeStr}'");
                        date = currentDate.Value;
                    }
                    else
                    {
                        // try parse first column as datetime
                        if (parts.Length > 0 && TryParseDateTime(parts[0], out date, out time))
                        {
                            // ok
                        }
                        else
                        {
                            throw new FormatException("No date/time column found");
                        }
                    }

                    static string CleanNumberString(string? s)
                    {
                        if (string.IsNullOrWhiteSpace(s)) return string.Empty;
                        s = s.Trim();
                        var arr = new System.Text.StringBuilder();
                        foreach (var ch in s)
                        {
                            if (char.IsDigit(ch) || ch == ',' || ch == '.' || ch == '-') arr.Append(ch);
                        }
                        return arr.ToString();
                    }

                    var weightClean = CleanNumberString(weightStr);
                    if (!decimal.TryParse(weightClean.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var weightKg))
                        throw new FormatException($"Invalid weight: '{weightStr}'");

                    decimal? changeKg = null;
                    if (!string.IsNullOrWhiteSpace(changeStr))
                    {
                        var chClean = CleanNumberString(changeStr);
                        if (decimal.TryParse(chClean.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var ch))
                            changeKg = ch;
                    }

                    decimal? bmi = null;
                    if (!string.IsNullOrWhiteSpace(bmiStr))
                    {
                        var bClean = CleanNumberString(bmiStr);
                        if (decimal.TryParse(bClean.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var b))
                            bmi = b;
                    }

                    decimal? fat = null;
                    if (!string.IsNullOrWhiteSpace(fatStr))
                    {
                        var fClean = CleanNumberString(fatStr);
                        if (decimal.TryParse(fClean.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var f))
                            fat = f;
                    }

                    decimal? muscle = null;
                    if (!string.IsNullOrWhiteSpace(muscleStr))
                    {
                        var mmClean = CleanNumberString(muscleStr);
                        if (decimal.TryParse(mmClean.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var mm))
                            muscle = mm;
                    }

                    decimal? water = null;
                    if (!string.IsNullOrWhiteSpace(waterStr))
                    {
                        var wClean = CleanNumberString(waterStr);
                        if (decimal.TryParse(wClean.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var w))
                            water = w;
                    }

                    MeasurementSource source = MeasurementSource.Unknown;
                    if (!string.IsNullOrWhiteSpace(sourceStr))
                    {
                        var s = sourceStr.Trim().ToLowerInvariant();
                        if (s.Contains("garmin")) source = MeasurementSource.GarminConnect;
                        else if (s.Contains("manual")) source = MeasurementSource.Manual;
                        else if (s.Contains("pro") || s.Contains("professional")) source = MeasurementSource.Professional;
                    }

                    var m = new WeightMeasurement
                    {
                        MeasuredDate = date,
                        MeasuredTime = time,
                        WeightKg = weightKg,
                        WeightChangeKg = changeKg,
                        BMI = bmi,
                        BodyFatPercentage = fat,
                        SkeletalMuscleMassKg = muscle,
                        BodyWaterPercentage = water,
                        Source = source,
                        SourceDetails = sourceStr,
                        Note = noteStr
                    };

                    list.Add(m);
                }
                catch (Exception ex)
                {
                    errors.Add(new ParseError { Line = lineNo, Message = ex.Message });
                }
            }

            return new ParseResult<WeightMeasurement> { Data = list, Errors = errors };
        }

        private static bool TryParseDateTime(string s, out DateOnly date, out TimeOnly time)
        {
            date = DateOnly.MinValue;
            time = TimeOnly.MinValue;
            if (string.IsNullOrWhiteSpace(s)) return false;

            s = s.Trim(' ', '"');
            // common formats: dd.MM.yyyy HH:mm:ss or yyyy-MM-dd HH:mm:ss or ISO
            var formats = new[] { "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd" };
            if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                date = DateOnly.FromDateTime(dt);
                time = TimeOnly.FromDateTime(dt);
                return true;
            }

            if (DateTime.TryParse(s, CultureInfo.GetCultureInfo("pl-PL"), DateTimeStyles.None, out dt))
            {
                date = DateOnly.FromDateTime(dt);
                time = TimeOnly.FromDateTime(dt);
                return true;
            }

            return false;
        }

        private static bool TryParseDateOnly(string s, out DateOnly date)
        {
            date = DateOnly.MinValue;
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim(' ', '"');
            if (DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) return true;
            if (DateTime.TryParse(s, CultureInfo.GetCultureInfo("pl-PL"), DateTimeStyles.None, out var dt))
            {
                date = DateOnly.FromDateTime(dt);
                return true;
            }
            // try common patterns like "yyyy MMM dd" where month is abbreviated (e.g. "2025 Gru 28")
            var formats = new[] { "yyyy MMM dd", "yyyy MMMM dd", "dd MMM yyyy" };
            if (DateTime.TryParseExact(s, formats, CultureInfo.GetCultureInfo("pl-PL"), DateTimeStyles.None, out dt))
            {
                date = DateOnly.FromDateTime(dt);
                return true;
            }
            return false;
        }

        private static bool TryParseTimeOnly(string s, out TimeOnly time)
        {
            time = TimeOnly.MinValue;
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim(' ', '"');
            if (TimeOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out time)) return true;
            if (TimeOnly.TryParse(s, new CultureInfo("en-US"), DateTimeStyles.None, out time)) return true;
            if (DateTime.TryParse(s, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out var dt))
            {
                time = TimeOnly.FromDateTime(dt);
                return true;
            }
            if (DateTime.TryParse(s, CultureInfo.GetCultureInfo("pl-PL"), DateTimeStyles.None, out dt))
            {
                time = TimeOnly.FromDateTime(dt);
                return true;
            }
            return false;
        }
    }
}
