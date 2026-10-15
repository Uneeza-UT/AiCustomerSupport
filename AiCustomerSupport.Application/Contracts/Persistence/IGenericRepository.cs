using System.Linq.Expressions;

namespace AiCustomerSupport.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task SaveChangesAsync();
    }
}
