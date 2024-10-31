using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Repository
{
    public class DirectorRepository : IAllDirectors
    {
        private readonly Db8428Context _context;
        public DirectorRepository(Db8428Context context)
        {
            _context = context;
        }

        public IEnumerable<Director> AllDirectors => _context.Directors;
    }
}
