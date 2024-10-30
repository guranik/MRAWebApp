using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewAggregatorWebApp.Model;
using Microsoft.EntityFrameworkCore;

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
                var dbContext = scope.ServiceProvider.GetRequiredService<Db8428Context>();

                // Кэширование данных из каждой таблицы последовательно
                await CacheData<Country>("countries", dbContext.Countries.Take(20).ToListAsync());
                await CacheData<Director>("directors", dbContext.Directors.Take(20).ToListAsync());
                await CacheData<Genre>("genres", dbContext.Genres.Take(20).ToListAsync());
                await CacheData<Movie>("movies", dbContext.Movies.Take(20).ToListAsync());
                await CacheData<Review>("reviews", dbContext.Reviews.Take(20).ToListAsync());
                await CacheData<User>("users", dbContext.Users.Take(20).ToListAsync());
                await CacheData<AllMovie>("allmovies", dbContext.AllMovies.Take(20).ToListAsync());
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