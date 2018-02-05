
namespace Chloe.Query.QueryState
{
    class GeneralQueryState : QueryStateBase, IQueryState
    {
        public GeneralQueryState(ResultElement resultElement)
            : base(resultElement)
        {
        }

        public override ResultElement ToFromQueryResult()
        {
            if (this.Result.Condition == null)
            {
                ResultElement result = new ResultElement(this.Result.ScopeParameters, this.Result.ScopeTables);
                result.FromTable = this.Result.FromTable;
                result.MappingObjectExpression = this.Result.MappingObjectExpression;
                return result;
            }

            return base.ToFromQueryResult();
        }

    }
}
