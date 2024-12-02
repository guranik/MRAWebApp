using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReviewAggregatorWebApp.Model;

public partial class Country
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название страны обязательно для заполнения.")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();

    public override string ToString()
    {
        return $"{Name}";
    }
}
