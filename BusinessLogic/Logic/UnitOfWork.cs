using BusinessLogic.Data;
using Core.Entities;
using Core.Interfaces;
using System.Collections;

namespace BusinessLogic.Logic
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MarketDbContext context;
        private Hashtable repository;

        public UnitOfWork(MarketDbContext context)
        {
            this.context = context;
        }

        public async Task<int> Complete()
        {
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : ClaseBase
        {
            if (repository is null)
            {
                repository = new Hashtable();

            }

            var type = typeof(TEntity).Name;
            if (!repository.Contains(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), context);
                repository.Add(type, repositoryInstance);

            }

            return (IGenericRepository<TEntity>)repository[type];
        }
    }
}
