using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthReport.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthReport.Infrastructure.Repositories
{
    public class Repository<T>(HealthReportDbContext db) : IRepository<T>
        where T : class
    {
        public IQueryable<T> Query() => db.Set<T>();

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await db.Set<T>().AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            // EF Core doesn't have an async AddRange, but AddRange is fast (in-memory).
            // Keep signature async to match repository contract and allow future async behavior.
            db.Set<T>().AddRange(entities);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            db.Set<T>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            db.Set<T>().UpdateRange(entities);
            await Task.CompletedTask;
        }

        public void Remove(T entity) => db.Set<T>().Remove(entity);

        public void RemoveRange(IEnumerable<T> entities)
        {
            db.Set<T>().RemoveRange(entities);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => db.SaveChangesAsync(cancellationToken);
    }
}
