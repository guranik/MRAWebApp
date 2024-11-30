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
        await databaseInitializer.InitializeAsync();
    }
    catch(Exception  ex){ }
}

// Использование middleware для кэширования
app.UseMiddleware<CachingMiddleware>();

// Использование сессий
app.UseSession();

var validTableNames = new HashSet<string> { };

// Middleware для обработки запросов
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    // Проверка на наличие имени таблицы в URL
    if (path.Contains("/"))
    {
        var segments = path.Split('/');
        var tableName = segments.Last().ToLower(); // Получаем последнее значение в URL как имя таблицы

        // Проверяем, является ли имя таблицы допустимым
        if (validTableNames.Contains(tableName))
        {
            // Получение данных из кэша
            if (context.RequestServices.GetService<IMemoryCache>().TryGetValue(tableName, out var cachedData))
            {
                // Приведение к IEnumerable<object> для вывода
                var dataList = cachedData as IEnumerable<object>;

                if (dataList != null)
                {
                    var htmlOutput = new StringBuilder();
                    htmlOutput.Append("<html<head><meta charset='UTF-8'></head>><body>");
                    htmlOutput.Append($"<h2>Data from {tableName} table:</h2>");
                    htmlOutput.Append("<ul>"); // Используем список для отображения объектов

                    // Проходим по каждому объекту и добавляем его ToString() в список
                    foreach (var item in dataList)
                    {
                        htmlOutput.Append($"<li>{item.ToString()}</li>");
                    }

                    htmlOutput.Append("</ul>");
                    htmlOutput.Append("</body></html>");

                    await context.Response.WriteAsync(htmlOutput.ToString());
                }
                else
                {
                    await context.Response.WriteAsync("<p>No data available</p>");
                }
            }
            else
            {
                await context.Response.WriteAsync($"No cached data found for table: {tableName}");
            }
            return; // Выход из конвейера обработки запроса
        }
    }


    if (path.Contains("/info"))
    {
        // Вывод информации о клиенте
        await context.Response.WriteAsync($"Client IP: {context.Connection.RemoteIpAddress}, User-Agent: {context.Request.Headers["User -Agent"]}");
        return; // Выход из конвейера обработки запроса
    }
    else if (path.Contains("/searchform1"))
    {
        if (context.Request.Method == "POST")
        {
            // Чтение данных из формы
            var firstname = context.Request.Form["firstname"];
            var lastname = context.Request.Form["lastname"];
            var options = context.Request.Form["options"];

            // Сохранение данных в куки
            context.Response.Cookies.Append("firstname", firstname);
            context.Response.Cookies.Append("lastname", lastname);
            context.Response.Cookies.Append("options", options);

            // Перенаправление на ту же страницу, чтобы обновить куки
            context.Response.Redirect("/searchform1");
            return;
        }

        // Чтение данных из куки для заполнения формы
        var savedFirstname = context.Request.Cookies["firstname"] ?? string.Empty;
        var savedLastname = context.Request.Cookies["lastname"] ?? string.Empty;
        var savedOption = context.Request.Cookies["options"] ?? string.Empty;

        // Выводим HTML формы для поиска с сохраненными значениями
        await context.Response.WriteAsync($@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                </head>
                <body>
                    <form method='post' action='/searchform1'>
                        First name:<br>
                        <input type='text' name='firstname' value='{savedFirstname}'><br>
                        Last name:<br>
                        <input type='text' name='lastname' value='{savedLastname}'><br>
                        Select an option:<br>
                        <select name='options'>
                            <option value='option1' {(savedOption == "option1" ? "selected" : "")}>Option 1</option>
                            <option value='option2' {(savedOption == "option2" ? "selected" : "")}>Option 2</option>
                        </select><br><br>
                        <input type='submit' value='Submit'>
                    </form>
                </body>
                </html>");
        return; // Выход из конвейера обработки запроса
    }
    else if (path.Contains("/searchform2"))
    {
        // Обработка POST-запроса для сохранения данных формы в сессии
        if (context.Request.Method == "POST")
        {
            var firstname = context.Request.Form["firstname"];
            var lastname = context.Request.Form["lastname"];
            var option = context.Request.Form["options"];

            // Создание объекта модели и сохранение в сессии
            var searchFormModel = new SearchFormModel
            {
                FirstName = firstname,
                LastName = lastname,
                SelectedOption = option
            };

            // Сериализация объекта модели в строку и сохранение в сессии
            context.Session.SetString("SearchFormModel", System.Text.Json.JsonSerializer.Serialize(searchFormModel));

            // Перенаправление на ту же страницу после сохранения
            context.Response.Redirect("/searchform2");
            return;
        }

        // Чтение данных из сессии для заполнения формы
        var sessionData = context.Session.GetString("SearchFormModel");
        var model = sessionData != null ? System.Text.Json.JsonSerializer.Deserialize<SearchFormModel>(sessionData) : new SearchFormModel();

        // Заполнение значениями из модели
        var savedFirstname = model.FirstName ?? string.Empty;
        var savedLastname = model.LastName ?? string.Empty;
        var savedOption = model.SelectedOption ?? string.Empty;

        // Выводим HTML формы для поиска с сохраненными значениями
        await context.Response.WriteAsync($@"
            <html>
            <body>
                <form method='post' action='/searchform2'>
                    First name:<br>
                    <input type='text' name='firstname' value='{savedFirstname}'><br>
                    Last name:<br>
                    <input type='text' name='lastname' value='{savedLastname}'><br>
                    Select an option:<br>
                    <select name='options'>
                        <option value='option1' {(savedOption == "option1" ? "selected" : "")}>Option 1</option>
                        <option value='option2' {(savedOption == "option2" ? "selected" : "")}>Option 2</option>
                    </select><br><br>
                    <input type='submit' value='Submit'>
                </form>
            </body>
            </html>");
        return; // Выход из конвейера обработки запроса
    }

    // Если ни один из условий не выполнен, передаем управление следующему компоненту в конвейере
    await next();
});

// Добавление маршрутизации MVC (если используется)
app.UseRouting();

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

app.Run();