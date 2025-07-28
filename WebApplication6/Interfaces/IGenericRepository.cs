using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication6.Interfaces
{
    // 'where T : class' kısıtlaması, bu interface'in sadece class'lar ile
    // (yani veritabanı modellerimiz ile) kullanılabilmesini sağlar.
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}