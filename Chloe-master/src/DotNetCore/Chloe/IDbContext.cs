using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Chloe
{
    public interface IDbContext : IDisposable
    {
        IDbSession Session { get; }

        IQuery<TEntity> Query<TEntity>();
        IQuery<TEntity> Query<TEntity>(string table);
        TEntity QueryByKey<TEntity>(object key, bool tracking = false);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key">If the entity just has one primary key, input a value that it's type is same as the primary key. If the entity has multiple keys, input an instance that defines the same properties as the keys like 'new { Key1 = "1", Key2 = "2" }'.</param>
        /// <param name="table"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        TEntity QueryByKey<TEntity>(object key, string table, bool tracking = false);

        /// <summary>
        /// context.JoinQuery&lt;User, City&gt;((user, city) => new object[] 
        /// { 
        ///     JoinType.LeftJoin, user.CityId == city.Id 
        /// })
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="joinInfo"></param>
        /// <returns></returns>
        IJoiningQuery<T1, T2> JoinQuery<T1, T2>(Expression<Func<T1, T2, object[]>> joinInfo);
        /// <summary>
        /// context.JoinQuery&lt;User, City, Province&gt;((user, city, province) => new object[] 
        /// { 
        ///     JoinType.LeftJoin, user.CityId == city.Id, 
        ///     JoinType.LeftJoin, city.ProvinceId == province.Id 
        /// })
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="joinInfo"></param>
        /// <returns></returns>
        IJoiningQuery<T1, T2, T3> JoinQuery<T1, T2, T3>(Expression<Func<T1, T2, T3, object[]>> joinInfo);
        IJoiningQuery<T1, T2, T3, T4> JoinQuery<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, object[]>> joinInfo);
        IJoiningQuery<T1, T2, T3, T4, T5> JoinQuery<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, object[]>> joinInfo);

        IEnumerable<T> SqlQuery<T>(string sql, params DbParam[] parameters);
        IEnumerable<T> SqlQuery<T>(string sql, CommandType cmdType, params DbParam[] parameters);

        TEntity Insert<TEntity>(TEntity entity);
        TEntity Insert<TEntity>(TEntity entity, string table);
        /// <summary>
        /// context.Insert&lt;User&gt;(() => new User() { Name = "lu", Age = 18, Gender = Gender.Woman, CityId = 1, OpTime = DateTime.Now })
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="content"></param>
        /// <returns>It will return null if an entity does not define primary key,other wise return primary key value.</returns>
        object Insert<TEntity>(Expression<Func<TEntity>> content);
        object Insert<TEntity>(Expression<Func<TEntity>> content, string table);
        /// <summary>
        /// 批量插入操作
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="keepIdentity">是否要把自增属性值插入到数据库</param>
        void InsertRange<TEntity>(List<TEntity> entities, bool keepIdentity = false);

        int Update<TEntity>(TEntity entity);
        int Update<TEntity>(TEntity entity, string table);
        /// <summary>
        /// context.Update&lt;User&gt;(a => a.Id == 1, a => new User() { Name = "lu", Age = a.Age + 1, Gender = Gender.Woman, OpTime = DateTime.Now })
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="condition"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        int Update<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content);
        int Update<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content, string table);

        int Delete<TEntity>(TEntity entity);
        int Delete<TEntity>(TEntity entity, string table);
        /// <summary>
        /// context.Delete&lt;User&gt;(a => a.Id == 1)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        int Delete<TEntity>(Expression<Func<TEntity, bool>> condition);
        int Delete<TEntity>(Expression<Func<TEntity, bool>> condition, string table);
        int DeleteByKey<TEntity>(object key);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key">If the entity just has one primary key, input a value that it's type is same as the primary key. If the entity has multiple keys, input an instance that defines the same properties as the keys like 'new { Key1 = "1", Key2 = "2" }'.</param>
        /// <param name="table"></param>
        /// <returns></returns>
        int DeleteByKey<TEntity>(object key, string table);

        void TrackEntity(object entity);
    }
}
