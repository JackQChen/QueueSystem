
namespace Chloe.DbExpressions
{
    [System.Diagnostics.DebuggerDisplay("Name = {Name}")]
    public class DbTable
    {
        string _name;
        string _schema;
        public DbTable(string name)
            : this(name, null)
        {
        }
        public DbTable(string name, string schema)
        {
            this._name = name;
            this._schema = schema;
        }

        public string Name { get { return this._name; } }
        public string Schema { get { return this._schema; } }
    }
}
