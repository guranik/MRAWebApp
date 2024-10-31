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
    }
}
