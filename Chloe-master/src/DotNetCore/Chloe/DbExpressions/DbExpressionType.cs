
namespace Chloe.DbExpressions
{
    public enum DbExpressionType
    {
        And = 1,
        Or,

        Equal,
        NotEqual,
        Not,

        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,

        Add,
        Subtract,
        Multiply,
        Divide,
        BitAnd,
        BitOr,
        Modulo,

        Negate,

        Convert,
        Constant,
        Coalesce,
        CaseWhen,
        MemberAccess,
        Call,

        Table,
        ColumnAccess,

        Parameter,
        FromTable,
        JoinTable,
        Aggregate,

        SqlQuery,
        SubQuery,
        Insert,
        Update,
        Delete,

        Exists,
    }
}
