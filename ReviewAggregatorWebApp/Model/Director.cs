using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReviewAggregatorWebApp.Model;

public partial class Director
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
