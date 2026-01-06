namespace HealthReport.Application.Interfaces
{

    public interface ITempFileStorage
    {
        Task<string> SaveAsync(Stream content, CancellationToken cancellationToken = default);
        Task<Stream> OpenReadAsync(string id, CancellationToken cancellationToken = default);
        Task<Stream> OpenWriteAsync(string id, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<string?> GetPathAsync(string id, CancellationToken cancellationToken = default);
        Task CleanupAsync(TimeSpan olderThan, CancellationToken cancellationToken = default);
    }
}
