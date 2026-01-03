using System.Text;

namespace HealthReport.Application.FileParsers;

public static class CsvHelper
{
    public static string[] SplitCsvLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return [];
        }
        
        var parts = new List<string>();
        var inQuotes = false;
        var cur = new StringBuilder();
        for (var i=0; i<line.Length; i++)
        {
            var c = line[i];
            switch (c)
            {
                case '"':
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        cur.Append('"');
                        i ++;
                        continue;
                    }
                    inQuotes = !inQuotes;
                    continue;
                }
                case ',' when !inQuotes:
                {
                    parts.Add(cur.ToString().Trim('"'));
                    cur.Clear();
                    continue;
                }
                default:
                {
                    cur.Append(c);
                    break;
                }
            }
        }

        if (inQuotes)
        {
            throw new FormatException();
        }
        parts.Add(cur.ToString().Trim('"'));
        return parts.ToArray();
    }
}