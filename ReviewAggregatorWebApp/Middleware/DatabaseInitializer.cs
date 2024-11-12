using Newtonsoft.Json;
using ReviewAggregatorWebApp.Model;
using ReviewAggregatorWebApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ReviewAggregatorWebApp.Middleware.ApiResponseData;
using ReviewAggregatorWebApp.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ReviewAggregatorWebApp.Middleware
{
    public class DatabaseInitializer
    {
        private readonly IAllMovies _movieRepository;
        private readonly IAllDirectors _directorRepository;
        private readonly IAllGenres _genreRepository;
        private readonly IAllCountries _countryRepository;
        private readonly HttpClient _httpClient;
        private readonly InitializationInfo _initInfo;


        private const string ApiKey = "PPQ11NW-5GX4PY1-Q7SFRZ1-Q64AJRD";
        private const string BaseUrl = "https://api.kinopoisk.dev";

        public DatabaseInitializer(IAllMovies movieRepository, IAllDirectors directorRepository,
                                    IAllGenres genreRepository, IAllCountries countryRepository, InitializationInfo initInfo)
        {
            _movieRepository = movieRepository;
            _directorRepository = directorRepository;
            _genreRepository = genreRepository;
            _countryRepository = countryRepository;

            _httpClient = new HttpClient();
            SetApiKey();
            _initInfo = initInfo;
        }

        private void SetApiKey()
        {
            string projectRoot = Directory.GetCurrentDirectory();
            while (projectRoot.Contains("bin"))
            {
                projectRoot = Directory.GetParent(projectRoot).FullName;
            }

            ConfigurationBuilder builder = new();

            builder.SetBasePath(projectRoot);

            builder.AddJsonFile("appsettings.json");

            IConfigurationRoot configuration = builder.AddUserSecrets<Program>().Build();

            string ApiKey = configuration["KinopoiskApiKey"];
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
        }

        public async Task InitializeAsync()
        {
            int startPage = ++_initInfo.LastProcessedPage;
            var movieIds = await GetMovieIdsAsync(startPage);
            if(_initInfo.LastRequestDate.Date != DateTime.Now.Date) _initInfo.RemainingRequests = 195;
         
            foreach (var movieId in movieIds)
            {
                var movie = await GetMovieDetailsAsync(movieId);
                UpdateInitializationInfo(startPage, --_initInfo.RemainingRequests, DateTime.Now);
                if (movie != null && (!string.IsNullOrEmpty(movie.Name) || !string.IsNullOrEmpty(movie.EnName) || !string.IsNullOrEmpty(movie.AlternativeName)))
                {
                    try
                    {
                        await SaveMovieAsync(movie);
                    }
                    catch (Exception) { }
                }
            }
        }

        private async Task<List<int>> GetMovieIdsAsync(int startPage = 1)
        {
            var movieIds = new List<int>();
            int currentPage = startPage;
            int endPage = startPage + 3;

            do
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/v1.4/movie?year=2020&limit=50&page={currentPage}");
                UpdateInitializationInfo(currentPage, --_initInfo.RemainingRequests, DateTime.Now);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                movieIds.AddRange(result.Docs.Select(m => m.Id));

                currentPage++;
            } while (currentPage <= endPage);

            return movieIds;
        }

        private void UpdateInitializationInfo(int lastProcessedPage, int remainingRequests, DateTime lastRequestDate)
        {
            _initInfo.LastProcessedPage = lastProcessedPage;
            _initInfo.RemainingRequests = remainingRequests;
            _initInfo.LastRequestDate = lastRequestDate;

            var configFile = "appsettings.json";
            var json = File.ReadAllText(configFile);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            jsonObj["InitializationInfo"]["LastProcessedPage"] = lastProcessedPage;
            jsonObj["InitializationInfo"]["RemainingRequests"] = remainingRequests;
            jsonObj["InitializationInfo"]["LastRequestDate"] = lastRequestDate.Date.ToString("yyyy-MM-ddTHH:mm:ss");

            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(configFile, output);
        }

        private async Task<MovieDetails> GetMovieDetailsAsync(int movieId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/v1.4/movie/{movieId}");
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MovieDetails>(jsonResponse);
        }

        private async Task SaveMovieAsync(MovieDetails movieDetails)
        {
            var director = await GetOrCreateDirectorAsync(movieDetails);
            var genres = await GetOrCreateGenresAsync(movieDetails.Genres);
            var countries = await GetOrCreateCountriesAsync(movieDetails.Countries);

            var movie = new Movie
            {
                Name = movieDetails.Name ?? movieDetails.AlternativeName ?? movieDetails.EnName,
                ReleaseDate = new DateTime(movieDetails.Year, 1, 1), // Установка года выпуска
                Rating = (decimal)(movieDetails.Rating.Kp != 0 ? movieDetails.Rating.Imdb : movieDetails.Rating.Kp),
                DirectorId = director != null? director.Id : null,
                Genres = genres,
                Countries = countries,
                PosterLink = movieDetails.Poster?.Url // Если есть, используем URL постера
            };

            _movieRepository.Create(movie);
        }

        private async Task<Director> GetOrCreateDirectorAsync(MovieDetails movieDetails)
        {
            var directorName = movieDetails.Persons.FirstOrDefault(p => p.Profession == "режиссеры")?.Name;
            if (string.IsNullOrEmpty(directorName))
            {
                directorName = movieDetails.Persons.FirstOrDefault(p => p.Profession == "режиссеры")?.EnName;
            }
            if (string.IsNullOrEmpty(directorName)) return null;

            var existingDirector = _directorRepository.AllDirectors.FirstOrDefault(d => d.Name == directorName);
            if (existingDirector != null) return existingDirector;

            var newDirector = new Director { Name = directorName };
            _directorRepository.Create(newDirector);
            return newDirector;
        }

        private async Task<List<Genre>> GetOrCreateGenresAsync(List<GenreInfo> genres)
        {
            var genreList = new List<Genre>();
            foreach (var genreInfo in genres)
            {
                var existingGenre = _genreRepository.AllGenres.FirstOrDefault(g => g.Name == genreInfo.Name);
                if (existingGenre != null)
                {
                    genreList.Add(existingGenre);
                }
                else
                {
                    var newGenre = new Genre { Name = genreInfo.Name };
                    _genreRepository.Create(newGenre);
                    genreList.Add(newGenre);
                }
            }
            return genreList;
        }

        private async Task<List<Country>> GetOrCreateCountriesAsync(List<CountryInfo> countries)
        {
            var countryList = new List<Country>();
            foreach (var countryInfo in countries)
            {
                var existingCountry = _countryRepository.AllCountries.FirstOrDefault(c => c.Name == countryInfo.Name);
                if (existingCountry != null)
                {
                    countryList.Add(existingCountry);
                }
                else
                {
                    var newCountry = new Country { Name = countryInfo.Name };
                    _countryRepository.Create(newCountry);
                    countryList.Add(newCountry);
                }
            }
            return countryList;
        }
    }
}