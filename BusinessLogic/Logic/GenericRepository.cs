using BusinessLogic.Data;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Logic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : ClaseBase
    {
        private readonly MarketDbContext context;

        public GenericRepository(MarketDbContext context)
        {
            this.context = context;
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }


        public async Task<T> GetByIdWithspec(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithspec(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> specification)
        {
            return SpecificationEvaluator<T>.GetQuery(context.Set<T>().AsQueryable(), specification);
        }

        public async Task<int> CountAsync(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).CountAsync();
        }

        public async Task<int> Add(T entity)
        {
            context.Set<T>().Add(entity);   
            return await context.SaveChangesAsync();
        }

        public async Task<int> Update(T entity)
        {
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            return await context.SaveChangesAsync();
        }

        public void AddEntity(T Entity)
        {
            context.Set<T>().Add(Entity);
        }

        public void UpdateEntity(T Entity)
        {
            context.Set<T>().Attach(Entity);
            context.Entry(Entity).State = EntityState.Modified;
        }

        public void DeleteEntity(T Entity)
        {
            context.Set<T>().Remove(Entity);
        }
    }
}
