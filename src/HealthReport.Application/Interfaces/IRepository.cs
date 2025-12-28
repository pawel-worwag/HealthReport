using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthReport.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query();
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        void Remove(T entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
