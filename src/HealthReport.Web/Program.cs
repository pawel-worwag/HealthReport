using HealthReport.Application.Interfaces;
using HealthReport.Infrastructure.TempFileStorage;
using HealthReport.Application.Services.ImportHandlers;
using HealthReport.Application.Services.Reports;
using HealthReport.Infrastructure;
using HealthReport.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

// Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();



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
