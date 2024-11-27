using ReviewAggregatorWebApp.Model;

namespace ReviewAggregatorWebApp.Interfaces
{
    public interface IAllYears
    {
        IEnumerable<int> AllYears { get; }
    }
}
