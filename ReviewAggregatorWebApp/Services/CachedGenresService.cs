using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces.Repositories;
using ReviewAggregatorWebApp.Interfaces.Services;
using ReviewAggregatorWebApp.Model;

namespace ReviewAggregatorWebApp.Services
{
    public class CachedGenresService : ICachedGenres
    {
        private readonly IAllGenres _genresRepository;
        private readonly IMemoryCache _cache;

        public CachedGenresService(IAllGenres genresRepository, IMemoryCache cache)
        {
            _genresRepository = genresRepository;
            _cache = cache;
        }

        public List<Genre> GetGenres()
        {
            List<Genre>? cachedGenres;

            if (!_cache.TryGetValue("genres", out cachedGenres))
            {
                var repositoryGenres = _genresRepository.AllGenres.ToList(); // Предполагается, что у вас есть метод GetPagedGenres

                _cache.Set("genres", repositoryGenres, TimeSpan.FromSeconds(256));
                return repositoryGenres;
            }
            return cachedGenres;
        }
    }
}