using System;
using System.Collections.Generic;

namespace ReviewAggregatorWebApp.Model;

public partial class MoviesFilteredByReleaseDate
{
    public string Режиссер { get; set; } = null!;

    public DateTime ДатаВыходаФильма { get; set; }

    public string НазваниеФильма { get; set; } = null!;

    public decimal Оценка { get; set; }
}
