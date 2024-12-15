using ReviewAggregatorWebApp.Model;

namespace ReviewAggregatorWebApp.Interfaces.Repositories
{
    public interface IAllYears
    {
        IEnumerable<int> AllYears { get; }
    }
}
