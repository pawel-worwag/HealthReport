using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using HealthReport.Infrastructure;
using HealthReport.Application.Interfaces;
using System.Linq;
using HealthReport.Infrastructure.TempFileStorage;
// Ensure import handlers namespace available (no-op if already present)
using HealthReport.Application.Services.ImportHandlers;
using HealthReport.Infrastructure.Repositories;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HealthReportDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register generic repository implementation
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Temp file storage configuration and registration
builder.Services.Configure<FileSystemTempFileStorageOptions>(builder.Configuration.GetSection("FileSystemTempFileStorage"));
builder.Services.AddSingleton<ITempFileStorage, FileSystemTempFileStorage>();

// Import handlers
builder.Services.AddScoped<IBloodPressureImportHandler, BloodPressureImportHandler>();
builder.Services.AddScoped<IBloodGlucoseImportHandler, BloodGlucoseImportHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

// Minimal API endpoint for blood pressure CSV import
app.MapPost("/api/import/bloodpressure", async (HttpRequest request, IBloodPressureImportHandler handler, CancellationToken ct) =>
{
    var form = await request.ReadFormAsync(ct).ConfigureAwait(false);
    var file = form.Files.GetFile("file");
    if (file == null || file.Length == 0)
        return Results.BadRequest(new { message = "No file uploaded" });

    await using var stream = file.OpenReadStream();
    var result = await handler.ImportAsync(stream, hasHeader: true, cancellationToken: ct).ConfigureAwait(false);
    return Results.Ok(new { imported = result.Data.Count(), errors = result.Errors });
});

// Minimal API endpoint for blood glucose CSV import
app.MapPost("/api/import/bloodglucose", async (HttpRequest request, IBloodGlucoseImportHandler handler, CancellationToken ct) =>
{
    var form = await request.ReadFormAsync(ct).ConfigureAwait(false);
    var file = form.Files.GetFile("file");
    if (file == null || file.Length == 0)
        return Results.BadRequest(new { message = "No file uploaded" });

    await using var stream = file.OpenReadStream();
    var result = await handler.ImportAsync(stream, hasHeader: true, cancellationToken: ct).ConfigureAwait(false);
    return Results.Ok(new { imported = result.Data.Count(), errors = result.Errors });
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
