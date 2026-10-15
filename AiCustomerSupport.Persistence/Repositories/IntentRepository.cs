using AiCustomerSupport.Application.Contracts.Persistence;
using AiCustomerSupport.Domain;
using AiCustomerSupport.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AiCustomerSupport.Persistence.Repositories
{
    public class IntentRepository : GenericRepository<Intent>, IIntentRepository
    {
        public IntentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<IReadOnlyList<Intent>> GetPagedAsync(int pageNumber = 1, int pageSize = 10)
        {
            int skip = (pageNumber - 1) * pageSize;

            return await _dbContext.Set<Intent>()
                .AsNoTracking()
                .OrderByDescending(i => i.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Intent>> SearchAsync(string searchTerm)
        {
            return await _dbContext.Set<Intent>()
                .AsNoTracking()
                .Where(i =>
                    i.Name.Contains(searchTerm) ||
                    i.Description.Contains(searchTerm))
                .ToListAsync();
        }


        public async Task<IReadOnlyList<Intent>> SortAsync<TKey>(
            bool descending, Expression<Func<Intent, TKey>> keySelector, int pageNumber, int pageSize)
        {
            var query = _dbContext.Set<Intent>().AsNoTracking();
            query = descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
