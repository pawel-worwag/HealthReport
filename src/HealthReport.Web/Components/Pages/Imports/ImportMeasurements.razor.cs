using HealthReport.Application.Contracts.Imports;
using HealthReport.Application.FileParsers;
using HealthReport.Application.Handlers.Imports;
using HealthReport.Application.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace HealthReport.Web.Components.Pages.Imports;

public partial class ImportMeasurements(ImportHandlerFactory importFactory, ITempFileStorage storage) : ComponentBase
{
    private readonly Dictionary<ImportSource, string> _allowedImports = new()
    {
        { ImportSource.ContourBloodGlucose , "Contour - Blood Glucose"},
        { ImportSource.IHealthBloodPressure , "IHealth - Blood Pressure"},
        { ImportSource.GarminWeight , "Garmin - Weight"}
    };

    private ImportSource? _selectedSource = ImportSource.ContourBloodGlucose;
    private IBrowserFile? _file;
    private string? _selectedFileName;
    private long _selectedFileSize;
    private CancellationTokenSource? _cts;
    private bool _isUploading = false;
    private ImportResultDto? _result = null;
    
    private void OnInputFileChange(InputFileChangeEventArgs e)
    {
        _file = e.File;
        _selectedFileName = _file?.Name;
        _selectedFileSize = _file?.Size ?? 0;
    }

    private async Task HandleValidSubmit()
    {
        if(_file is null || _selectedSource is null) return;
        
        var maxAllowed = 1024L * 1024 * 200; // 200 MB
        if (_file.Size > maxAllowed)
        {
            Console.WriteLine("File too big. Max allowed is 200 MB.");
            return;
        }
        
        try
        {
            _isUploading = true;
            _cts = new CancellationTokenSource();
            
            var id = Guid.NewGuid().ToString();
            await using var sourceStream = _file.OpenReadStream(maxAllowed, _cts.Token);
            await using var output = await storage.OpenWriteAsync(id, _cts.Token);
            await sourceStream.CopyToAsync(output, 4096, _cts.Token);
            sourceStream.Close();
            output.Close();

            await using var uploaded = await storage.OpenReadAsync(id, _cts.Token);
            
            var handler =  importFactory.Create(_selectedSource.Value);
            _result = await handler.ImportAsync(uploaded,true, _cts.Token);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            _isUploading = false;
            _cts?.Dispose();
            _cts = null;
            StateHasChanged();
        }
    }
}