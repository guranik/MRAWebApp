using Microsoft.EntityFrameworkCore;
using ReviewAggregatorWebApp.Model;
using ReviewAggregatorWebApp.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReviewAggregatorWebApp.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Repository;
using Microsoft.Extensions.Configuration;
using ReviewAggregatorWebApp.Middleware.ApiResponseData;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

// Подключение сервисов
builder.Services.AddDbContext<Db8428Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RemoteConnection")));
builder.Services.Configure<InitializationInfo>(builder.Configuration.GetSection("InitializationInfo"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<InitializationInfo>>().Value);
builder.Services.AddTransient<IAllUsers, UserRepository>();
builder.Services.AddTransient<IAllDirectors, DirectorRepository>();
builder.Services.AddTransient<IAllGenres, GenreRepository>();
builder.Services.AddTransient<IAllCountries, CountryRepository>();
builder.Services.AddTransient<IAllMovies, MovieRepository>();
builder.Services.AddTransient<IAllReviews, ReviewRepository>();
builder.Services.AddTransient<IAllYears, YearRepository>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<Db8428Context>()
.AddDefaultTokenProviders();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var movieRepository = services.GetRequiredService<IAllMovies>();
    var directorRepository = services.GetRequiredService<IAllDirectors>();
    var genreRepository = services.GetRequiredService<IAllGenres>();
    var countryRepository = services.GetRequiredService<IAllCountries>();
    var initializationInfo = services.GetRequiredService<InitializationInfo>();

    var databaseInitializer = new DatabaseInitializer(movieRepository, directorRepository, genreRepository, countryRepository, initializationInfo);
    try
    {
        await databaseInitializer.CreateRoles(services); // Вызов метода создания 
    }
    catch (Exception  ex){ }
    try
    {
        await databaseInitializer.InitializeAsync();
    }
    catch (Exception ex) { }

}

// Использование middleware для кэширования
app.UseMiddleware<CachingMiddleware>();
app.UseHttpsRedirection();

app.UseStaticFiles();

// Использование сессий
app.UseSession();

// Добавление маршрутизации MVC (если используется)
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Filter",
    pattern: "Filter/{genre?}/{year?}/{director?}/{country?}/{sortBy?}",
    defaults: new { controller = "MoviesList", action = "Filter" }
);

app.MapControllerRoute(
    name: "movieDetails",
    pattern: "MovieInfo/Details/{id}",
    defaults: new { controller = "MovieInfo", action = "Details" });

app.MapControllerRoute(
    name: "Countries",
    pattern: "countries/{action=Index}/{id?}",
    defaults: new { controller = "Countries" });

app.MapControllerRoute(
    name: "Genres",
    pattern: "genres/{action=Index}/{id?}",
    defaults: new { controller = "Genres" });

app.MapControllerRoute(
    name: "Years",
    pattern: "years/{action=Index}/{id?}",
    defaults: new { controller = "Years" });

app.MapControllerRoute(
    name: "Directors",
    pattern: "directors/{action=Index}/{id?}",
    defaults: new { controller = "Directors" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Genres}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "reviews",
    pattern: "Reviews/{action=Index}/{id?}",
    defaults: new { controller = "Reviews" });

app.Run();