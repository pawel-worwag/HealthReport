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
                lineNo++;
            }

            while (!reader.EndOfStream)
            {
                lineNo++;
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = CsvHelper.SplitCsvLine(line);
                // Expected: #,Data i godzina,BGValue[mg/dl],Znacznik posiłku,Źródło danych,Uwagi,Aktywność,Posiłek[g],Leki,Lokalizacja
                if (parts.Length != 10)
                {
                    errors.Add(new ParseError { Line = lineNo, Message = $"Invalid number of columns: {parts.Length}" });
                    continue;
                }

                try
                {
                    DateOnly? date = null;
                    TimeOnly? time = null;
                    int? glucose = null;
                    MealMarker? mealMarker = null;
                    string? note = null;
                    
                    //date and time
                    var dateTimeStr = parts[1].Trim();
                    if (DateTime.TryParseExact(dateTimeStr, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                    {
                        date = DateOnly.FromDateTime(dt);
                        time = TimeOnly.FromDateTime(dt);
                    }
                    else
                    {
                        errors.Add(new ParseError { Line = lineNo, Message = $"Invalid datetime format: '{dateTimeStr}'" });
                        break;
                    }
                    
                    //BGValue
                    if (int.TryParse(parts[2], out var bgValue))
                    {
                        glucose = bgValue;
                    }
                    else
                    {
                        errors.Add(new ParseError { Line = lineNo, Message = $"Invalid BGValue format: '{parts[2]}'" });
                        break;
                    }
                    
                    //MealMarker
                    mealMarker = MapMealMarker(parts[3]);
                    
                    //Note
                    
                    note = parts[5].Trim();

                    var m = new BloodGlucoseMeasurement
                    {
                        MeasuredDate = (DateOnly)date,
                        MeasuredTime = (TimeOnly)time,
                        BGValue = (int)glucose,
                        Meal = (MealMarker)mealMarker,
                        Note = string.IsNullOrWhiteSpace(note) ? null : note
                    };

                }
                catch (Exception ex)
                {
                    errors.Add(new ParseError { Line = lineNo, Message = ex.Message });
                }


            }

            return new ParseResult<BloodGlucoseMeasurement> { Data = measurements, Errors = errors };

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
                _ => MealMarker.Unknown
            };
        }
    }
}
