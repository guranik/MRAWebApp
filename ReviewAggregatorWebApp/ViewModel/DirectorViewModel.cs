using System.ComponentModel.DataAnnotations;

namespace ReviewAggregatorWebApp.ViewModel
{
    public class DirectorViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Имя режиссера обязательно для заполнения.")]
        public string Name { get; set; }
        public bool IsEditing { get; set; }
    }
}
