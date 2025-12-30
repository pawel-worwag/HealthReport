using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthReport.Application.Extensions;

public static class EnumerableExtensions
{
    // Int
    public static int? MinOrNull(this IEnumerable<int>? source)
    {
        if (source == null) return null;
        return source.Any() ? (int?)source.Min() : null;
    }

    public static int? MaxOrNull(this IEnumerable<int>? source)
    {
        if (source == null) return null;
        return source.Any() ? (int?)source.Max() : null;
    }

    public static decimal? AverageOrNull(this IEnumerable<int>? source)
    {
        if (source == null) return null;
        if (!source.Any()) return null;
        return Convert.ToDecimal(source.Average());
    }

    // Nullable int
    public static int? MinOrNull(this IEnumerable<int?>? source)
    {
        if (source == null) return null;
        var filtered = source.Where(x => x.HasValue).Select(x => x!.Value);
        return filtered.Any() ? (int?)filtered.Min() : null;
    }

    public static int? MaxOrNull(this IEnumerable<int?>? source)
    {
        if (source == null) return null;
        var filtered = source.Where(x => x.HasValue).Select(x => x!.Value);
        return filtered.Any() ? (int?)filtered.Max() : null;
    }

    public static decimal? AverageOrNull(this IEnumerable<int?>? source)
    {
        if (source == null) return null;
        var filtered = source.Where(x => x.HasValue).Select(x => x!.Value);
        if (!filtered.Any()) return null;
        return Convert.ToDecimal(filtered.Average());
    }

    // Decimal
    public static decimal? MinOrNull(this IEnumerable<decimal>? source)
    {
        if (source == null) return null;
        return source.Any() ? (decimal?)source.Min() : null;
    }

    public static decimal? MaxOrNull(this IEnumerable<decimal>? source)
    {
        if (source == null) return null;
        return source.Any() ? (decimal?)source.Max() : null;
    }

    public static decimal? AverageOrNull(this IEnumerable<decimal>? source)
    {
        if (source == null) return null;
        return source.Any() ? (decimal?)source.Average() : null;
    }

    // Nullable decimal
    public static decimal? MinOrNull(this IEnumerable<decimal?>? source)
    {
        if (source == null) return null;
        var filtered = source.Where(x => x.HasValue).Select(x => x!.Value);
        return filtered.Any() ? (decimal?)filtered.Min() : null;
    }

    public static decimal? MaxOrNull(this IEnumerable<decimal?>? source)
    {
        if (source == null) return null;
        var filtered = source.Where(x => x.HasValue).Select(x => x!.Value);
        return filtered.Any() ? (decimal?)filtered.Max() : null;
    }

    public static decimal? AverageOrNull(this IEnumerable<decimal?>? source)
    {
        if (source == null) return null;
        var filtered = source.Where(x => x.HasValue).Select(x => x!.Value);
        return filtered.Any() ? (decimal?)filtered.Average() : null;
    }

    // Selector overloads for arbitrary sequences
    public static decimal? AverageOrNull<T>(this IEnumerable<T>? source, Func<T, decimal> selector)
    {
        if (source == null) return null;
        var projected = source.Select(selector);
        return projected.Any() ? (decimal?)projected.Average() : null;
    }

    public static decimal? AverageOrNull<T>(this IEnumerable<T>? source, Func<T, int> selector)
    {
        if (source == null) return null;
        var projected = source.Select(selector);
        if (!projected.Any()) return null;
        return Convert.ToDecimal(projected.Average());
    }

    public static decimal? MinOrNull<T>(this IEnumerable<T>? source, Func<T, decimal> selector)
    {
        if (source == null) return null;
        var projected = source.Select(selector);
        return projected.Any() ? (decimal?)projected.Min() : null;
    }

    public static decimal? MaxOrNull<T>(this IEnumerable<T>? source, Func<T, decimal> selector)
    {
        if (source == null) return null;
        var projected = source.Select(selector);
        return projected.Any() ? (decimal?)projected.Max() : null;
    }
}
