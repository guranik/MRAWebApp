using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewAggregatorWebApp.Model;
using Microsoft.EntityFrameworkCore;
using ReviewAggregatorWebApp.Interfaces;

namespace ReviewAggregatorWebApp.Middleware
{
    public class CachingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly IServiceScopeFactory _scopeFactory;

        public CachingMiddleware(RequestDelegate next, IMemoryCache cache, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _cache = cache;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Создаем новый скоуп
            using (var scope = _scopeFactory.CreateScope())
            {
                var movieRepository = scope.ServiceProvider.GetRequiredService<IAllMovies>();
                var countryRepository = scope.ServiceProvider.GetRequiredService<IAllCountries>();
                var directorRepository = scope.ServiceProvider.GetRequiredService<IAllDirectors>();
                var genreRepository = scope.ServiceProvider.GetRequiredService<IAllGenres>();
                var yearRepository = scope.ServiceProvider.GetRequiredService<IAllYears>();

                // Кэширование данных из каждой таблицы последовательно
                await CacheData("countries", countryRepository.AllCountries.AsQueryable().Take(60).ToListAsync());
                await CacheData("directors", directorRepository.AllDirectors.AsQueryable().Take(300).ToListAsync());
                await CacheData("genres", genreRepository.AllGenres.AsQueryable().Take(60).ToListAsync());
                await CacheData("movies", movieRepository.AllMovies.AsQueryable().Take(500).ToListAsync());
                await CacheData("years", Task.FromResult(yearRepository.AllYears.ToList()));
            }

            await _next(context); // Передаем управление следующему компоненту
        }

        private async Task CacheData<T>(string key, Task<List<T>> dataTask)
        {
            if (!_cache.TryGetValue(key, out _))
            {
                var data = await dataTask;
                var cacheDuration = TimeSpan.FromSeconds(256);
                _cache.Set(key, data, cacheDuration);
            }
        }
    }
}