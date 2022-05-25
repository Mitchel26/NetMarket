using Core.Specifications;
using Microsoft.AspNetCore.Identity;

namespace Core.Interfaces
{
    public interface IGenericSeguridadRepository<T> where T : IdentityUser
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);


        Task<T> GetByIdWithspec(ISpecification<T> specification);
        Task<IReadOnlyList<T>> GetAllWithspec(ISpecification<T> specification);
        Task<int> CountAsync(ISpecification<T> specification);

        Task<int> Add(T entity);
        Task<int> Update(T entity);
    }
}
