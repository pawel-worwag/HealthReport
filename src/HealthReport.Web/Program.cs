using Microsoft.EntityFrameworkCore;
using HealthReport.Infrastructure;
using HealthReport.Application.Interfaces;
using HealthReport.Infrastructure.TempFileStorage;
using HealthReport.Application.Services.ImportHandlers;
using HealthReport.Infrastructure.Repositories;
using HealthReport.Application.Services.Reports;
using HealthReport.Web.Extensions;

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
builder.Services.AddScoped<IWeightImportHandler, WeightImportHandler>();
// Reports
builder.Services.AddScoped<IMonthlyReportHandler, MonthlyReportHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapEndpoint();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
