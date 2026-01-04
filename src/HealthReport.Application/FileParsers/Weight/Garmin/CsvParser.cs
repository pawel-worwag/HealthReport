using System.Globalization;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.FileParsers.Weight.Garmin
{
    /// <summary>
    /// Simple CSV importer for Garmin Connect weight exports.
    /// It expects a header row and columns containing at least: Date, Time, Weight(kg).
    /// The importer is tolerant: missing optional columns (BMI, fat, muscle, water, change) are allowed.
    /// </summary>
    public static class CsvParser
    {
        public static CsvParsingResult<WeightMeasurement> ParseCsv(Stream csvStream, bool hasHeader = true)
        {
            var measurments = new List<WeightMeasurement>();
            var errors = new List<ParsingError>();

            using var reader = new StreamReader(csvStream);
            int lineNo = 0;

            if (hasHeader)
            {
                reader.ReadLine();
                lineNo++;
            }

            DateOnly? measuredDate = null;
            while (!reader.EndOfStream)
            {
                lineNo++;
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                var parts = CsvHelper.SplitCsvLine(line);
                try
                {
                    if (parts.Length != 2 && parts.Length != 9)
                    {
                        errors.Add(new ParsingError
                            { Line = lineNo, Message = $"Invalid number of columns: {parts.Length}" });
                        break;
                    }

                    if (parts.Length == 2) // Date
                    {
                        if (DateOnly.TryParse(parts[0].Trim(' '), new CultureInfo("pl-PL"), out var parsedDate))
                        {
                            measuredDate = parsedDate;
                        }
                        else
                        {
                            errors.Add(new ParsingError { Line = lineNo, Message = $"Invalid date format: value='{parts[0]}'" });
                            break;
                        }
                    }
                    
                    TimeOnly? measuredTime = null;
                    decimal? measuredWeightKg = null;
                    decimal? measuredWeightChangeKg = null;
                    decimal? measuredBmi = null;
                    decimal? measuredBodyFatPercentage = null;
                    decimal? measuredSkeletalMuscleMassKg = null;
                    decimal? measuredBodyWaterPercentage = null;
                    MeasurementSource? source = null;
                    string? sourceDetails = null;
                    string? note = null;

                    if (parts.Length == 9) // Record
                    {
                        if (measuredDate is null)
                        {
                            errors.Add(new ParsingError
                                { Line = lineNo, Message = $"No date record first" });
                            break;
                        }
                        
                        if (TimeOnly.TryParse(parts[0].Trim(' '), CultureInfo.InvariantCulture, out var parsedTime))
                        {
                            measuredTime = parsedTime;
                        }
                        else
                        {
                            errors.Add(new ParsingError { Line = lineNo, Message = $"Invalid time format: value='{parts[0]}'" });
                            break;
                        }

                        if (decimal.TryParse(parts[1], out var weightKg))
                        {
                            measuredWeightKg = weightKg;
                        }
                        else
                        {
                            errors.Add(new ParsingError { Line = lineNo, Message = $"Invalid decimal format: value='{parts[1]}'" });
                            break;
                        }
                        
                        if(decimal.TryParse(parts[2], out var weightChangeKg))
                        {
                            measuredWeightChangeKg= weightChangeKg;
                        }
                        else
                        {
                            errors.Add(new ParsingError { Line = lineNo, Message = $"Invalid decimal format: value='{parts[1]}'" });
                            break;
                        }

                        if (decimal.TryParse(parts[3], out var bmi))
                        {
                            measuredBmi = bmi;
                        }
                        else
                        {
                            errors.Add(new ParsingError { Line = lineNo, Message = $"Invalid decimal format: value='{parts[1]}'" });
                        }

                        if (decimal.TryParse(parts[4], out var bodyFatPercentage))
                        {
                            measuredBodyFatPercentage = bodyFatPercentage;
                        }
                        else
                        {
                            errors.Add(new ParsingError { Line = lineNo, Message = $"Invalid decimal format: value='{parts[1]}'" });
                        }

                        if (decimal.TryParse(parts[5], out var skeletalMuscleMassKg))
                        {
                            measuredSkeletalMuscleMassKg = skeletalMuscleMassKg;
                        }
                        else
                        {
                            errors.Add(new ParsingError { Line = lineNo, Message = $"Invalid decimal format: value='{parts[1]}'" });
                        }

                        if (decimal.TryParse(parts[7], out var bodyWaterPercentage))
                        {
                            measuredBodyWaterPercentage = bodyWaterPercentage;
                        }
                        else
                        {
                            errors.Add(new ParsingError { Line = lineNo, Message = $"Invalid decimal format: value='{parts[1]}'" });
                        }
                        
                        measurments.Add(new WeightMeasurement()
                        {
                            MeasuredDate = (DateOnly)measuredDate,
                            MeasuredTime = (TimeOnly)measuredTime,
                            WeightKg = (Decimal)measuredWeightKg,
                            WeightChangeKg = (Decimal)measuredWeightChangeKg,
                            BMI = measuredBmi,
                            BodyFatPercentage = measuredBodyFatPercentage,
                            SkeletalMuscleMassKg = measuredSkeletalMuscleMassKg,
                            BodyWaterPercentage = measuredBodyWaterPercentage,
                            Note = note,
                            Source = MeasurementSource.GarminConnect,
                            SourceDetails = "CSV"
                        });
                    }
                    
                }
                catch (Exception ex)
                {
                    errors.Add(new ParsingError { Line = lineNo, Message = ex.Message });
                }
            }

            return new CsvParsingResult<WeightMeasurement>() { Data = measurments, Errors = errors };
        }
    }
}
