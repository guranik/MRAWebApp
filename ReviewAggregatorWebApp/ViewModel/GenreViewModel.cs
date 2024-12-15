using System.ComponentModel.DataAnnotations;

namespace ReviewAggregatorWebApp.ViewModel
{
    public class GenreViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Название жанра обязательно для заполнения.")]
        public string Name { get; set; }
        public bool IsEditing { get; set; }
    }
}
