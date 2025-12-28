namespace HealthReport.Infrastructure.TempFileStorage
{
    public class FileSystemTempFileStorageOptions
    {
        /// <summary>
        /// Base directory for temporary files. If empty, the system temporary directory is used.
        /// </summary>
        public string? BasePath { get; set; }
    }
}
