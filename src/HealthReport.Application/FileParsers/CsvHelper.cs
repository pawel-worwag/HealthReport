using System.Text;

namespace HealthReport.Application.FileParsers;

public static class CsvHelper
{
    private static string[] SplitCsvLine(string line)
    {
        var parts = new List<string>();
        var inQuotes = false;
        var cur = new StringBuilder();
        foreach (var c in line)
        {
            switch (c)
            {
                case '"':
                    inQuotes = !inQuotes;
                    continue;
                case ',' when !inQuotes:
                    parts.Add(cur.ToString().Trim('"'));
                    cur.Clear();
                    continue;
                default:
                    cur.Append(c);
                    break;
            }
        }
        parts.Add(cur.ToString().Trim('"'));
        return parts.ToArray();
    }
}