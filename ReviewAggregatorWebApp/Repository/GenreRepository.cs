using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Repository
{
    public class GenreRepository : IAllGenres
    {
        private readonly Db8428Context _context;
        public GenreRepository(Db8428Context context)
        {
            _context = context;
        }

        public IEnumerable<Genre> AllGenres => _context.Genres;

        public Genre GetById(int id) => _context.Genres.Find(id)
            ?? throw new InvalidOperationException($"Genre with ID {id} not found.");

        public void Create(Genre genre)
        {
            _context.Genres.Add(genre);
            _context.SaveChanges();
        }

        public void Update(Genre genre)
        {
            _context.Genres.Update(genre);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var genre = GetById(id);
            _context.Genres.Remove(genre);
            _context.SaveChanges();
        }
    }
}
