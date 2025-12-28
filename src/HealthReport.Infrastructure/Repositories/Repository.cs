using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthReport.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthReport.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly HealthReportDbContext _db;

        public Repository(HealthReportDbContext db)
        {
            _db = db;
        }

        public IQueryable<T> Query() => _db.Set<T>();

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            // Use synchronous Add to simplify behavior; DbContext will track entity.
            _db.Set<T>().Add(entity);
            await Task.CompletedTask;
        }

        public void Remove(T entity) => _db.Set<T>().Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _db.SaveChangesAsync(cancellationToken);
    }
}
