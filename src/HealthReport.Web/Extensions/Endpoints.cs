using HealthReport.Application.Contracts.Imports;
using HealthReport.Application.Contracts.Reports;
using HealthReport.Application.Errors;
using HealthReport.Application.Handlers.Imports;
using HealthReport.Application.Handlers.Reports.Simple;
using HealthReport.Identity.Application;
using HealthReport.Identity.Contracts;
using Scalar.AspNetCore;

namespace HealthReport.Web.Extensions;

public static class Endpoints
{
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapGet("/api/error-test", ()=>
        {
            throw new Exception("Test exception");
        });
        
        // Minimal API endpoint for blood pressure CSV import
        app.MapPost("/api/import/bloodpressure", async (HttpRequest request, IBloodPressureImportHandler handler, CancellationToken ct) =>
        {
            var form = await request.ReadFormAsync(ct).ConfigureAwait(false);
            var file = form.Files.GetFile("file");
            if (file == null || file.Length == 0)
                return Results.BadRequest(new { message = "No file uploaded" });

            await using var stream = file.OpenReadStream();
            return Results.Ok(await handler.ImportAsync(stream, hasHeader: true, cancellationToken: ct));
        })
        .WithTags("Imports")
        .Produces<ImportResultDto>(StatusCodes.Status200OK,"application/json")
        .Produces<ApiError>(StatusCodes.Status400BadRequest, "application/json")
        .Produces<ApiError>(StatusCodes.Status500InternalServerError, "application/json");
        
        // Minimal API endpoint for blood glucose CSV import
        app.MapPost("/api/import/bloodglucose", async (HttpRequest request, IBloodGlucoseImportHandler handler, CancellationToken ct) =>
        {
            var form = await request.ReadFormAsync(ct).ConfigureAwait(false);
            var file = form.Files.GetFile("file");
            if (file == null || file.Length == 0)
                return Results.BadRequest(new { message = "No file uploaded" });

            await using var stream = file.OpenReadStream();
            return Results.Ok(await handler.ImportAsync(stream, hasHeader: true, cancellationToken: ct));
        })
        .WithTags("Imports")
        .Produces<ImportResultDto>(StatusCodes.Status200OK,"application/json")
        .Produces<ApiError>(StatusCodes.Status400BadRequest, "application/json")
        .Produces<ApiError>(StatusCodes.Status500InternalServerError, "application/json");
        
        // Minimal API endpoint for weight CSV import
        app.MapPost("/api/import/weight", async (HttpRequest request, IWeightImportHandler handler, CancellationToken ct) =>
        {
            var form = await request.ReadFormAsync(ct).ConfigureAwait(false);
            var file = form.Files.GetFile("file");
            if (file == null || file.Length == 0)
                return Results.BadRequest(new { message = "No file uploaded" });

            await using var stream = file.OpenReadStream();
            return Results.Ok(await handler.ImportAsync(stream, hasHeader: true, cancellationToken: ct));
        })
        .WithTags("Imports")
        .Produces<ImportResultDto>(StatusCodes.Status200OK,"application/json")
        .Produces<ApiError>(StatusCodes.Status400BadRequest, "application/json")
        .Produces<ApiError>(StatusCodes.Status500InternalServerError, "application/json");

        // Simple report endpoint
        app.MapGet("/api/reports/simple", async (ISimpleReportHandler handler, int? year, int? month, CancellationToken ct) =>
        {
            if (!year.HasValue || !month.HasValue)
                return Results.BadRequest(new { message = "Please provide year and month query parameters, e.g. ?year=2025&month=12" });

            if (month < 1 || month > 12)
                return Results.BadRequest(new { message = "Month must be between 1 and 12" });

            var report = handler.GenerateMonthlyReport(year.Value, month.Value);
            return Results.Ok(report);
        })
        .WithTags("Reports")
        .Produces<SimpleReportDto>(StatusCodes.Status200OK,"application/json")
        .Produces<ApiError>(StatusCodes.Status400BadRequest, "application/json")
        .Produces<ApiError>(StatusCodes.Status500InternalServerError, "application/json");
        
        app.MapGet("/api/reports/simple/xlsx", async (ISimpleReportHandler handler, int? year, int? month, CancellationToken ct) =>
        {
            if (!year.HasValue || !month.HasValue)
                return Results.BadRequest(new { message = "Please provide year and month query parameters, e.g. ?year=2025&month=12" });

            if (month < 1 || month > 12)
                return Results.BadRequest(new { message = "Month must be between 1 and 12" });

            var report = handler.GenerateMonthlyReport(year.Value, month.Value);

            return Results.File(SimpleReportToXlsx.Export(report),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"simple_report_{year.Value}_{month.Value}.xlsx");
        })
        .WithTags("Reports")
        .Produces<byte[]>(StatusCodes.Status200OK ,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        .Produces<ApiError>(StatusCodes.Status400BadRequest, "application/json")
        .Produces<ApiError>(StatusCodes.Status500InternalServerError, "application/json");
        
        return app;
    }
}