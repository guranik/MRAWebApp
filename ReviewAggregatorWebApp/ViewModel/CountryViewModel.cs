using System.ComponentModel.DataAnnotations;

namespace ReviewAggregatorWebApp.ViewModel
{
    public class CountryViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Название страны обязательно для заполнения.")]
        public string Name { get; set; }
        public bool IsEditing { get; set; }
    }
}
