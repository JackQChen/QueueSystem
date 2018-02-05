using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Chloe.Extension;

namespace Chloe
{
    public static class DbContextAsyncExtension
    {
        public static Task<TEntity> QueryByKeyAsync<TEntity>(this IDbContext dbContext, object key, bool tracking = false)
        {
            return Utils.MakeTask(() => dbContext.QueryByKey<TEntity>(key, tracking));
        }

        public static Task<List<T>> SqlQueryAsync<T>(this IDbContext dbContext, string sql, params DbParam[] parameters)
        {
            return Utils.MakeTask(() => dbContext.SqlQuery<T>(sql, parameters).ToList());
        }
        public static Task<List<T>> SqlQueryAsync<T>(this IDbContext dbContext, string sql, CommandType cmdType, params DbParam[] parameters)
        {
            return Utils.MakeTask(() => dbContext.SqlQuery<T>(sql, cmdType, parameters).ToList());
        }

        public static Task<TEntity> InsertAsync<TEntity>(this IDbContext dbContext, TEntity entity)
        {
            return Utils.MakeTask(() => dbContext.Insert<TEntity>(entity));
        }
        public static Task<object> InsertAsync<TEntity>(this IDbContext dbContext, Expression<Func<TEntity>> body)
        {
            return Utils.MakeTask(() => dbContext.Insert<TEntity>(body));
        }

        public static Task<int> UpdateAsync<TEntity>(this IDbContext dbContext, TEntity entity)
        {
            return Utils.MakeTask(() => dbContext.Update<TEntity>(entity));
        }
        public static Task<int> UpdateAsync<TEntity>(this IDbContext dbContext, Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> body)
        {
            return Utils.MakeTask(() => dbContext.Update<TEntity>(condition, body));
        }

        public static Task<int> DeleteAsync<TEntity>(this IDbContext dbContext, TEntity entity)
        {
            return Utils.MakeTask(() => dbContext.Delete<TEntity>(entity));
        }
        public static Task<int> DeleteAsync<TEntity>(this IDbContext dbContext, Expression<Func<TEntity, bool>> condition)
        {
            return Utils.MakeTask(() => dbContext.Delete<TEntity>(condition));
        }
        public static Task<int> DeleteByKeyAsync<TEntity>(this IDbContext dbContext, object key)
        {
            return Utils.MakeTask(() => dbContext.DeleteByKey<TEntity>(key));
        }
    }
}
