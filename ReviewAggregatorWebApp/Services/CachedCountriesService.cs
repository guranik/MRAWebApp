using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Interfaces.Repositories;
using ReviewAggregatorWebApp.Interfaces.Services;
using ReviewAggregatorWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Services
{
    public class CachedCountriesService : ICachedCountries
    {
        private readonly IAllCountries _countriesRepository;
        private readonly IMemoryCache _cache;

        public CachedCountriesService(IAllCountries countriesRepository, IMemoryCache cache)
        {
            _countriesRepository = countriesRepository;
            _cache = cache;
        }

        public PagedList<Country> GetCountries(int pageNumber, int pageSize)
        {
            if(pageNumber == 1)
            {
                List<Country>? cachedCountries;

                if (!_cache.TryGetValue("pagedCountries", out cachedCountries))
                {
                    var repositoryCountries = _countriesRepository.GetPagedCountries(1, pageSize);

                    _cache.Set("pagedCountries", repositoryCountries.Items, TimeSpan.FromSeconds(256));
                    return repositoryCountries;
                }

                var pagedCountries = new PagedList<Country>(cachedCountries, _countriesRepository.GetTotalCount(), pageNumber, pageSize);
                return pagedCountries;
            }
            else
            {
                return _countriesRepository.GetPagedCountries(pageNumber, pageSize);
            }     
        }
    }
}