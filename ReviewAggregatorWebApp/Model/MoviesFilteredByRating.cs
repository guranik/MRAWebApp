using System;
using System.Collections.Generic;

namespace ReviewAggregatorWebApp.Model;

public partial class MoviesFilteredByRating
{
    public string НазваниеФильма { get; set; } = null!;

    public string Режиссер { get; set; } = null!;

    public DateTime ДатаВыхода { get; set; }

    public decimal Оценка { get; set; }
}
