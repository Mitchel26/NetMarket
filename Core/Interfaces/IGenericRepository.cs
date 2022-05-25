using Core.Entities;
using Core.Specifications;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : ClaseBase
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);


        Task<T> GetByIdWithspec(ISpecification<T> specification);
        Task<IReadOnlyList<T>> GetAllWithspec(ISpecification<T> specification);
        Task<int> CountAsync(ISpecification<T> specification);

        Task<int> Add(T entity);
        Task<int> Update(T entity);

        void AddEntity(T Entity);
        void UpdateEntity(T Entity);
        void DeleteEntity(T Entity);

    }
}
