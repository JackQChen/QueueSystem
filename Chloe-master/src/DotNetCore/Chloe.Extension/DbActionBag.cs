using Chloe.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Chloe
{
    /// <summary>
    /// 暂存 CURD 操作，最后调用 ExecuteActions 执行。
    /// </summary>
    public class DbActionBag
    {
        List<Func<int>> _actions = new List<Func<int>>();
        IDbContext _dbContext;

        public DbActionBag(IDbContext dbContext)
        {
            Utils.CheckNull(dbContext);
            this._dbContext = dbContext;
        }

        public void PushInsert<T>(T entity)
        {
            this._actions.Add(() =>
            {
                this._dbContext.Insert(entity);
                return 1;
            });
        }
        public void PushInsert<T>(Expression<Func<T>> body)
        {
            this._actions.Add(() =>
            {
                this._dbContext.Insert(body);
                return 1;
            });
        }
        public void PushUpdate<T>(T entity)
        {
            this._actions.Add(() =>
            {
                return this._dbContext.Update(entity);
            });
        }
        public void PushUpdate<T>(Expression<Func<T, bool>> condition, Expression<Func<T, T>> body)
        {
            this._actions.Add(() =>
            {
                return this._dbContext.Update(condition, body);
            });
        }
        public void PushDelete<T>(T entity)
        {
            this._actions.Add(() =>
            {
                return this._dbContext.Delete(entity);
            });
        }
        public void PushDelete<T>(Expression<Func<T, bool>> condition)
        {
            this._actions.Add(() =>
            {
                return this._dbContext.Delete(condition);
            });
        }

        public void Push(Func<IDbContext, int> action)
        {
            this._actions.Add(() =>
            {
                return action(this._dbContext);
            });
        }

        public int ExecuteActions()
        {
            if (this._actions.Count == 0)
                return 0;

            int affected = 0;

            if (this._dbContext.Session.IsInTransaction == true)
            {
                affected = this.InnerExecuteActions();
                this._actions.Clear();
                return affected;
            }


            affected = this._dbContext.DoWithTransaction(() =>
                {
                    return this.InnerExecuteActions();
                }, IsolationLevel.ReadCommitted);

            this._actions.Clear();

            return affected;
        }


        int InnerExecuteActions()
        {
            int affected = 0;
            for (int i = 0; i < this._actions.Count; i++)
            {
                Func<int> action = this._actions[i];
                affected += action();
            }

            return affected;
        }
    }
}
