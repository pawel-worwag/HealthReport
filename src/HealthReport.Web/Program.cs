using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using HealthReport.Infrastructure;
using HealthReport.Application.Interfaces;
using HealthReport.Infrastructure.TempFileStorage;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HealthReportDbContext>(options =>
    options.UseNpgsql(connectionString));

// Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Temp file storage configuration and registration
builder.Services.Configure<FileSystemTempFileStorageOptions>(builder.Configuration.GetSection("FileSystemTempFileStorage"));
builder.Services.AddSingleton<ITempFileStorage, FileSystemTempFileStorage>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
