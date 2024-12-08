using Microsoft.AspNetCore.Mvc.Rendering;
using ReviewAggregatorWebApp.Model;
using System.ComponentModel.DataAnnotations;

namespace ReviewAggregatorWebApp.ViewModel
{
    public class MovieViewModel
    {
        public int Id { get; set; } // Идентификатор фильма

        [Required(ErrorMessage = "Название фильма обязательно для заполнения.")]
        public string Name { get; set; } = null!; // Название фильма

        public string? PosterLink { get; set; } // Ссылка на постер фильма

        [Required(ErrorMessage = "Выберите режиссера.")]
        public int? DirectorId { get; set; } // Идентификатор режиссера

        [Required(ErrorMessage = "Дата выпуска обязательна для заполнения.")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; } // Дата выпуска фильма

        [Required(ErrorMessage = "Рейтинг обязателен для заполнения.")]
        [Range(0, 10, ErrorMessage = "Рейтинг должен быть в диапазоне от 0 до 10.")]
        public decimal Rating { get; set; } // Рейтинг фильма

        // Списки для выбора
        public List<int> CountryIds { get; set; } = new List<int>(); // Список идентификаторов стран
        public List<int> GenreIds { get; set; } = new List<int>(); // Список идентификаторов жанров

        // Для отображения списков в выпадающих списках
        public SelectList Directors { get; set; } = new SelectList(new List<Director>()); // Список режиссеров
        public SelectList Genres { get; set; } = new SelectList(new List<Genre>()); // Список жанров
        public SelectList Countries { get; set; } = new SelectList(new List<Country>()); // Список стран

        // Дополнительное свойство для проверки режима редактирования
        public bool IsEditing { get; set; } // Указывает, редактируется ли фильм
    }
}
