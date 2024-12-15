using ReviewAggregatorWebApp.Model;

namespace ReviewAggregatorWebApp.Interfaces.Services
{
    public interface ICachedGenres
    {
        public List<Genre> GetGenres();
    }
}
