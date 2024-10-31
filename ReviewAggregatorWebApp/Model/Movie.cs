using System;
using System.Collections.Generic;

namespace ReviewAggregatorWebApp.Model;

public partial class Movie
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string PosterLink { get; set; } = null!;

    public int DirectorId { get; set; }

    public DateTime ReleaseDate { get; set; }

    public decimal Rating { get; set; }

    public virtual Director Director { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Country> Countries { get; set; } = new List<Country>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    public override string ToString()
    {
        string Genres = string.Join(", ", this.Genres.Select(obj => obj.ToString()));
        string Countries = string.Join(", ", this.Countries.Select(obj => obj.ToString()));
        return $"{Name}: released in {ReleaseDate}, by director: {Director}, has rating: {Rating}. Genres: {Genres}. Countries of action: {Countries}";
    }
}
