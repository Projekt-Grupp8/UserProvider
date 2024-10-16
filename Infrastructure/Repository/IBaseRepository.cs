using Infrastructure.Models;
using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<ResponseResult> CreateAsync(TEntity entity);
        Task<ResponseResult> DeleteOneAsync(Expression<Func<TEntity, bool>> predicate);
        Task<ResponseResult> Exists(Expression<Func<TEntity, bool>> predicate);
        Task<ResponseResult> GetAllAsync();
        Task<ResponseResult> GetOneAsync(Expression<Func<TEntity, bool>> predicate);
        Task<ResponseResult> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity newEntity);
    }
}