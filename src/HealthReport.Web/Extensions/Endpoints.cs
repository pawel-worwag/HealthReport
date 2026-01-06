using System.Security.Claims;
using HealthReport.Application.Errors;
using HealthReport.Application.Handlers.Reports.Simple;
using HealthReport.Application.Handlers.Reports.SimpleAvg;
using HealthReport.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthReport.Web.Extensions;

public static class Endpoints
{
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapGet("/api/reports/simple/xlsx", async (ISimpleReportHandler handler, int? year, int? month, ClaimsPrincipal user, CancellationToken ct) =>
        {
            if (user?.Identity is null || !user.Identity.IsAuthenticated)
                return Results.Unauthorized();

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Results.Forbid();
            
            if (!year.HasValue || !month.HasValue)
                return Results.BadRequest(new { message = "Please provide year and month query parameters, e.g. ?year=2025&month=12" });

            if (month < 1 || month > 12)
                return Results.BadRequest(new { message = "Month must be between 1 and 12" });

            var report = handler.GenerateMonthlyReport(year.Value, month.Value, Guid.Parse(userId));

            return Results.File(SimpleReportToXlsx.Export(report),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"simple_report_{year.Value}_{month.Value}.xlsx");
        })
        .WithTags("Reports")
        .Produces<byte[]>(StatusCodes.Status200OK ,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        .Produces<ApiError>(StatusCodes.Status400BadRequest, "application/json")
        .Produces<ApiError>(StatusCodes.Status500InternalServerError, "application/json")
        .RequireAuthorization();
        
        
        
        app.MapGet("/api/reports/simple-avg/xlsx", async (ISimpleAvhReportHandler handler, int fromYear, int fromMonth, int toYear, int toMonth, ClaimsPrincipal user, CancellationToken ct) =>
            {
                if (user?.Identity is null || !user.Identity.IsAuthenticated)
                    return Results.Unauthorized();

                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? user.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Results.Forbid();
                
                if (fromMonth < 1 || fromMonth > 12  || toMonth < 1 || toMonth > 12)
                    return Results.BadRequest(new { message = "Month must be between 1 and 12" });
                
                var report = handler.GenerateReport(fromYear, fromMonth, toYear, toMonth, Guid.Parse(userId));
                
                return Results.File(SimpleAvgReportToXlsx.Export(report),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"simple_report_{fromYear}_{fromMonth}-{toYear}_{toMonth}.xlsx");
            })
            .WithTags("Reports")
            .Produces<byte[]>(StatusCodes.Status200OK ,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            .Produces<ApiError>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ApiError>(StatusCodes.Status500InternalServerError, "application/json")
            .RequireAuthorization();
        
        
        
        
        
        return app;
    }
}