using Core.Specifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Data
{
    public class SeguridadSpecificationEvaluator<T> where T : IdentityUser
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            // Agrega los criterios
            if (specification.Criteria != null)
            {
                inputQuery = inputQuery.Where(specification.Criteria);
            }
            // Orden
            if (specification.OrderBy != null)
            {
                inputQuery = inputQuery.OrderBy(specification.OrderBy);
            }

            if (specification.OrderByDescending != null)
            {
                inputQuery = inputQuery.OrderByDescending(specification.OrderByDescending);
            }

            // Paginación
            if (specification.IsPagingEnable)
            {
                inputQuery = inputQuery.Skip(specification.Skip).Take(specification.Take);
            }

            // Agrega los includes
            inputQuery = specification.Includes.Aggregate(inputQuery, (current, include) => current.Include(include));



            return inputQuery;

        }
    }
}
