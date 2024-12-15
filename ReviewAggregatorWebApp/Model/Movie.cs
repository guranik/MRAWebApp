using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReviewAggregatorWebApp.Model;
public partial class Movie
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название фильма обязательно для заполнения.")]
    public string Name { get; set; } = null!;

    public string? PosterLink { get; set; }

    [Required(ErrorMessage = "Выберите режиссера.")]
    public int? DirectorId { get; set; }

    [Required(ErrorMessage = "Дата выпуска обязательна для заполнения.")]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

    [Required(ErrorMessage = "Рейтинг обязателен для заполнения.")]
    [Range(0, 10, ErrorMessage = "Рейтинг должен быть в диапазоне от 0 до 10.")]
    public decimal KpRating { get; set; }
    public decimal? Rating { get; set; }

    public virtual Director? Director { get; set; }

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
