using Microsoft.AspNetCore.Mvc.Rendering;
using ReviewAggregatorWebApp.Model;
using System.ComponentModel.DataAnnotations;

namespace ReviewAggregatorWebApp.ViewModel
{
    public class MovieViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название фильма обязательно для заполнения.")]
        public string Name { get; set; } = null!;

        public string? PosterLink { get; set; }

        [Required(ErrorMessage = "Выберите режиссера.")]
        public int? DirectorId { get; set; }
        public Director? Director { get; set; }

        [Required(ErrorMessage = "Дата выпуска обязательна для заполнения.")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }


        public List<int> CountryIds { get; set; } = new List<int>(); // Список идентификаторов стран
        public List<int> GenreIds { get; set; } = new List<int>(); // Список идентификаторов жанров

        public SelectList Directors { get; set; } = new SelectList(new List<Director>()); // Список режиссеров
        public SelectList Genres { get; set; } = new SelectList(new List<Genre>()); // Список жанров
        public SelectList Countries { get; set; } = new SelectList(new List<Country>()); // Список стран

        public bool IsEditing { get; set; } // Указывает, редактируется ли фильм
    }
}
