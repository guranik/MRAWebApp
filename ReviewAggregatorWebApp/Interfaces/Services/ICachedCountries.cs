using ReviewAggregatorWebApp.Model;

namespace ReviewAggregatorWebApp.Interfaces.Services
{
    public interface ICachedCountries
    {
        public PagedList<Country> GetCountries(int pageNumber, int pageSize);
    }
}
