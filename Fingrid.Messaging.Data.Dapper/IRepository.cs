using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Data.Dapper
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
        Task<IEnumerable<T>> GetTopNAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "", int noOfItems = 0);

        Task<T> GetByIdAsync(long id);

        Task<int> InsertAsync(T entity);

        Task<int> DeleteAsync(T entity);

        Task DeleteByIdAsync(long id);

        Task<int> UpdateAsync(T entity);

    }
}
