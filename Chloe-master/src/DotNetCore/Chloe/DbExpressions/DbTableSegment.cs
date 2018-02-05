
namespace Chloe.DbExpressions
{
    /// <summary>
    /// User as T1 , (select * from User) as T1
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Alias = {Alias}")]
    public class DbTableSegment
    {
        DbExpression _body;
        string _alias;

        public DbTableSegment(DbExpression body, string alias)
        {
            this._body = body;
            this._alias = alias;
        }

        /// <summary>
        /// User、(select * from User)
        /// </summary>
        public DbExpression Body { get { return this._body; } }
        public string Alias { get { return this._alias; } }
    }
}
