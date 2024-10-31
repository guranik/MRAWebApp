using System;
using System.Collections.Generic;

namespace ReviewAggregatorWebApp.Model;

public partial class Review
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int MovieId { get; set; }

    public DateTime PostDate { get; set; }

    public int Rating { get; set; }

    public string ReviewText { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public override string ToString()
    {
        return $"Review of movie: {Movie.Name}. Author of review: {User.Login}. Date of review: {PostDate}. Review text: {ReviewText}. Rating: {Rating}";
    }
}
