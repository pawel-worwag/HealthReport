namespace HealthReport.Application.Interfaces
{
    /// <summary>
    /// Simple interface for saving and reading temporary files.
    /// Implementations should store files and return an identifier (e.g. GUID).
    /// </summary>
    public interface ITempFileStorage
    {
        /// <summary>
        /// Saves the content of the provided stream to a temporary file. Returns the file identifier.
        /// </summary>
        Task<string> SaveAsync(Stream content, string? extension = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Opens a read-only stream for the stored file identified by <paramref name="id"/>.
        /// </summary>
        Task<Stream> OpenReadAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the file with the given identifier (if it exists).
        /// </summary>
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the full file path on disk if the file exists; otherwise null.
        /// </summary>
        Task<string?> GetPathAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes files older than the specified timespan (best-effort).
        /// </summary>
        Task CleanupAsync(TimeSpan olderThan, CancellationToken cancellationToken = default);
    }
}
