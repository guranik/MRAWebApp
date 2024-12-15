using ReviewAggregatorWebApp.Interfaces.Repositories;
using ReviewAggregatorWebApp.Model;

namespace ReviewAggregatorWebApp.Repository
{
    public class YearRepository : IAllYears
    {
        private readonly List<int> _years;

        public YearRepository()
        {
            _years = new List<int>();
            InitializeYears();
        }

        public IEnumerable<int> AllYears => _years;

        private void InitializeYears()
        {
            for (int year = DateTime.Now.Year - 10; year <= DateTime.Now.Year; year++)
            {
                _years.Add(year);
            }
        }
    }

}
