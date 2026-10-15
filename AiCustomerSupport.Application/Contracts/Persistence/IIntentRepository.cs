using AiCustomerSupport.Domain;
using System.Linq.Expressions;

namespace AiCustomerSupport.Application.Contracts.Persistence
{
    public interface IIntentRepository : IGenericRepository<Intent>
    {
        Task<IReadOnlyList<Intent>> GetPagedAsync(int pageNumber = 1, int pageSize = 10);
        Task<IReadOnlyList<Intent>> SearchAsync(string searchTerm);
        Task<IReadOnlyList<Intent>> SortAsync<TKey>(
            bool descending, Expression<Func<Intent, TKey>> keySelector, int pageNumber, int pageSize);
    }
}
