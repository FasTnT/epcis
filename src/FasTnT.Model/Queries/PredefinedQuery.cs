namespace FasTnT.Model.Queries
{
    public abstract class PredefinedQuery : EpcisQuery
    {
        public abstract bool AllowsSubscription { get; }
    }
}
