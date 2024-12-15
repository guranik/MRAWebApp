using ReviewAggregatorWebApp.Model;

namespace ReviewAggregatorWebApp.Interfaces.Services
{
    public interface ICachedDirectors
    {
        public PagedList<Director> GetDirectors(int pageNumber, int pageSize);
    }
}
