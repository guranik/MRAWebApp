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
                var userRepository = scope.ServiceProvider.GetRequiredService<IAllUsers>();
                var movieRepository = scope.ServiceProvider.GetRequiredService<IAllMovies>();
                var countryRepository = scope.ServiceProvider.GetRequiredService<IAllCountries>();
                var directorRepository = scope.ServiceProvider.GetRequiredService<IAllDirectors>();
                var genreRepository = scope.ServiceProvider.GetRequiredService<IAllGenres>();
                var reviewRepository = scope.ServiceProvider.GetRequiredService<IAllReviews>();

                // Кэширование данных из каждой таблицы последовательно
                await CacheData("countries", countryRepository.AllCountries.AsQueryable().Take(20).ToListAsync());
                await CacheData("directors", directorRepository.AllDirectors.AsQueryable().Take(20).ToListAsync());
                await CacheData("genres", genreRepository.AllGenres.AsQueryable().Take(20).ToListAsync());
                await CacheData("movies", movieRepository.AllMovies.AsQueryable().Take(20).ToListAsync());
                await CacheData("reviews", reviewRepository.AllReviews.AsQueryable().Take(20).ToListAsync());
                await CacheData("users", userRepository.AllUsers.AsQueryable().Take(20).ToListAsync());
            }

            await _next(context); // Передаем управление следующему компоненту
        }

        private async Task CacheData<T>(string key, Task<List<T>> dataTask)
        {
            if (!_cache.TryGetValue(key, out _))
            {
                var data = await dataTask;
                var cacheDuration = TimeSpan.FromSeconds(256); // N - номер варианта
                _cache.Set(key, data, cacheDuration);
            }
        }
    }
}