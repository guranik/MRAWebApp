using System;
using System.Collections.Generic;

namespace ReviewAggregatorWebApp.Model;

public partial class AllMovie
{
    public decimal Рейтинг { get; set; }

    public DateTime ДатаВыхода { get; set; }

    public string НазваниеФильма { get; set; } = null!;

    public string Режиссер { get; set; } = null!;

    public override string ToString()
    {
        return $"{Режиссер}: {НазваниеФильма}, {ДатаВыхода}, {Рейтинг}";
    }
}
