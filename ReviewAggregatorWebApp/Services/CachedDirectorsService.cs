using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces.Repositories;
using ReviewAggregatorWebApp.Interfaces.Services;
using ReviewAggregatorWebApp.Model;

namespace ReviewAggregatorWebApp.Services
{
    public class CachedDirectorsService : ICachedDirectors
    {
        private readonly IAllDirectors _directorsRepository;
        private readonly IMemoryCache _cache;

        public CachedDirectorsService(IAllDirectors directorsRepository, IMemoryCache cache)
        {
            _directorsRepository = directorsRepository;
            _cache = cache;
        }

        public PagedList<Director> GetDirectors(int pageNumber, int pageSize)
        {
            if (pageNumber == 1)
            {
                List<Director>? cachedDirectors;

                if (!_cache.TryGetValue("directors", out cachedDirectors))
                {
                    var repositoryDirectors = _directorsRepository.GetPagedDirectors(1, pageSize);

                    _cache.Set("directors", repositoryDirectors.Items, TimeSpan.FromSeconds(256));
                    return repositoryDirectors;
                }

                var pagedDirectors = new PagedList<Director>(cachedDirectors, _directorsRepository.GetTotalCount(), pageNumber, pageSize);
                return pagedDirectors;
            }
            else
            {
                return _directorsRepository.GetPagedDirectors(pageNumber, pageSize);
            }
        }
    }
}
