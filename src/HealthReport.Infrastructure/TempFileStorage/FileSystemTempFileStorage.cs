using System.Runtime.InteropServices;
using HealthReport.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace HealthReport.Infrastructure.TempFileStorage
{
    public class FileSystemTempFileStorage(IOptions<FileSystemTempFileStorageOptions> options) : ITempFileStorage
    {
        private readonly string _basePath = options.Value.BasePath??Path.GetTempPath();

        public async Task<string> SaveAsync(Stream content, CancellationToken cancellationToken = default)
        {
            var id = Guid.NewGuid().ToString("N");
            var path = ResolvePath(id);

            await using var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096,
                useAsync: true);
            await content.CopyToAsync(fs, cancellationToken).ConfigureAwait(false);
            await fs.FlushAsync(cancellationToken).ConfigureAwait(false);

            return id;
        }

        public Task<Stream> OpenReadAsync(string id, CancellationToken cancellationToken = default)
        {
            var path = ResolvePath(id);
            if (!File.Exists(path)) throw new FileNotFoundException("Temporary file not found", id);
            Stream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            return Task.FromResult(fs);
        }

        public Task<Stream> OpenWriteAsync(string id, CancellationToken cancellationToken = default)
        {
            var path = ResolvePath(id);
            if (File.Exists(path)) throw new FileNotFoundException("File already exists", id);
            Stream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Write, 4096,
                useAsync: true);
            return Task.FromResult(fs);
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var path = ResolvePath(id);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return Task.CompletedTask;
        }

        public Task<string?> GetPathAsync(string id, CancellationToken cancellationToken = default)
        {
            var path = ResolvePath(id);
            return File.Exists(path)
                ? Task.FromResult<string?>(Path.GetFullPath(path))
                : Task.FromResult<string?>(null);
        }

        public Task CleanupAsync(TimeSpan olderThan, CancellationToken cancellationToken = default)
        {
            var threshold = DateTimeOffset.UtcNow - olderThan;
            var dir = new DirectoryInfo(_basePath);
            if (!dir.Exists) return Task.CompletedTask;

            var files = dir.GetFiles()
                .Where(f => f.CreationTimeUtc < threshold.UtcDateTime || f.LastWriteTimeUtc < threshold.UtcDateTime)
                .ToArray();

            foreach (var f in files)
            {
                try
                {
                    f.Delete();
                }
                catch
                {
                    /* best-effort cleanup */
                }
            }

            return Task.CompletedTask;
        }

        private string ResolvePath(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"Null or whitespaces", nameof(id));
            }

            if (id.IndexOfAny([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, '\0']) >= 0 
                || id == "." 
                || id == ".." 
                || id.Contains("../") 
                || id.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new ArgumentException("Invalid file name");

            var baseFull = Path.GetFullPath(_basePath);
            baseFull = Path.TrimEndingDirectorySeparator(baseFull);
            var candidate = Path.GetFullPath(Path.Combine(baseFull, id));
            
            var cmp = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            
            if (!string.Equals(candidate, baseFull, cmp) && !candidate.StartsWith(baseFull + Path.DirectorySeparatorChar, cmp))
                throw new ArgumentException("Path traversal detected");
            
            return candidate;
        }
    }
}